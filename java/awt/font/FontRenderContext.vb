Imports System

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

'
' * @author Charlton Innovations, Inc.
' 

Namespace java.awt.font


	''' <summary>
	'''   The <code>FontRenderContext</code> class is a container for the
	'''   information needed to correctly measure text.  The measurement of text
	'''   can vary because of rules that map outlines to pixels, and rendering
	'''   hints provided by an application.
	'''   <p>
	'''   One such piece of information is a transform that scales
	'''   typographical points to pixels. (A point is defined to be exactly 1/72
	'''   of an inch, which is slightly different than
	'''   the traditional mechanical measurement of a point.)  A character that
	'''   is rendered at 12pt on a 600dpi device might have a different size
	'''   than the same character rendered at 12pt on a 72dpi device because of
	'''   such factors as rounding to pixel boundaries and hints that the font
	'''   designer may have specified.
	'''   <p>
	'''   Anti-aliasing and Fractional-metrics specified by an application can also
	'''   affect the size of a character because of rounding to pixel
	'''   boundaries.
	'''   <p>
	'''   Typically, instances of <code>FontRenderContext</code> are
	'''   obtained from a <seealso cref="java.awt.Graphics2D Graphics2D"/> object.  A
	'''   <code>FontRenderContext</code> which is directly constructed will
	'''   most likely not represent any actual graphics device, and may lead
	'''   to unexpected or incorrect results. </summary>
	'''   <seealso cref= java.awt.RenderingHints#KEY_TEXT_ANTIALIASING </seealso>
	'''   <seealso cref= java.awt.RenderingHints#KEY_FRACTIONALMETRICS </seealso>
	'''   <seealso cref= java.awt.Graphics2D#getFontRenderContext() </seealso>
	'''   <seealso cref= java.awt.font.LineMetrics </seealso>

	Public Class FontRenderContext
		<NonSerialized> _
		Private tx As java.awt.geom.AffineTransform
		<NonSerialized> _
		Private aaHintValue As Object
		<NonSerialized> _
		Private fmHintValue As Object
		<NonSerialized> _
		Private defaulting As Boolean

		''' <summary>
		''' Constructs a new <code>FontRenderContext</code>
		''' object.
		''' 
		''' </summary>
		Protected Friend Sub New()
			aaHintValue = VALUE_TEXT_ANTIALIAS_DEFAULT
			fmHintValue = VALUE_FRACTIONALMETRICS_DEFAULT
			defaulting = True
		End Sub

		''' <summary>
		''' Constructs a <code>FontRenderContext</code> object from an
		''' optional <seealso cref="AffineTransform"/> and two <code>boolean</code>
		''' values that determine if the newly constructed object has
		''' anti-aliasing or fractional metrics.
		''' In each case the boolean values <CODE>true</CODE> and <CODE>false</CODE>
		''' correspond to the rendering hint values <CODE>ON</CODE> and
		''' <CODE>OFF</CODE> respectively.
		''' <p>
		''' To specify other hint values, use the constructor which
		''' specifies the rendering hint values as parameters :
		''' <seealso cref="#FontRenderContext(AffineTransform, Object, Object)"/>. </summary>
		''' <param name="tx"> the transform which is used to scale typographical points
		''' to pixels in this <code>FontRenderContext</code>.  If null, an
		''' identity transform is used. </param>
		''' <param name="isAntiAliased"> determines if the newly constructed object
		''' has anti-aliasing. </param>
		''' <param name="usesFractionalMetrics"> determines if the newly constructed
		''' object has fractional metrics. </param>
		Public Sub New(  tx As java.awt.geom.AffineTransform,   isAntiAliased As Boolean,   usesFractionalMetrics As Boolean)
			If tx IsNot Nothing AndAlso (Not tx.identity) Then Me.tx = New java.awt.geom.AffineTransform(tx)
			If isAntiAliased Then
				aaHintValue = VALUE_TEXT_ANTIALIAS_ON
			Else
				aaHintValue = VALUE_TEXT_ANTIALIAS_OFF
			End If
			If usesFractionalMetrics Then
				fmHintValue = VALUE_FRACTIONALMETRICS_ON
			Else
				fmHintValue = VALUE_FRACTIONALMETRICS_OFF
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>FontRenderContext</code> object from an
		''' optional <seealso cref="AffineTransform"/> and two <code>Object</code>
		''' values that determine if the newly constructed object has
		''' anti-aliasing or fractional metrics. </summary>
		''' <param name="tx"> the transform which is used to scale typographical points
		''' to pixels in this <code>FontRenderContext</code>.  If null, an
		''' identity transform is used. </param>
		''' <param name="aaHint"> - one of the text antialiasing rendering hint values
		''' defined in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>.
		''' Any other value will throw <code>IllegalArgumentException</code>.
		''' <seealso cref="java.awt.RenderingHints#VALUE_TEXT_ANTIALIAS_DEFAULT VALUE_TEXT_ANTIALIAS_DEFAULT"/>
		''' may be specified, in which case the mode used is implementation
		''' dependent. </param>
		''' <param name="fmHint"> - one of the text fractional rendering hint values defined
		''' in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>.
		''' <seealso cref="java.awt.RenderingHints#VALUE_FRACTIONALMETRICS_DEFAULT VALUE_FRACTIONALMETRICS_DEFAULT"/>
		''' may be specified, in which case the mode used is implementation
		''' dependent.
		''' Any other value will throw <code>IllegalArgumentException</code> </param>
		''' <exception cref="IllegalArgumentException"> if the hints are not one of the
		''' legal values.
		''' @since 1.6 </exception>
		Public Sub New(  tx As java.awt.geom.AffineTransform,   aaHint As Object,   fmHint As Object)
			If tx IsNot Nothing AndAlso (Not tx.identity) Then Me.tx = New java.awt.geom.AffineTransform(tx)
			Try
				If KEY_TEXT_ANTIALIASING.isCompatibleValue(aaHint) Then
					aaHintValue = aaHint
				Else
					Throw New IllegalArgumentException("AA hint:" & aaHint)
				End If
			Catch e As Exception
				Throw New IllegalArgumentException("AA hint:" & aaHint)
			End Try
			Try
				If KEY_FRACTIONALMETRICS.isCompatibleValue(fmHint) Then
					fmHintValue = fmHint
				Else
					Throw New IllegalArgumentException("FM hint:" & fmHint)
				End If
			Catch e As Exception
				Throw New IllegalArgumentException("FM hint:" & fmHint)
			End Try
		End Sub

		''' <summary>
		''' Indicates whether or not this <code>FontRenderContext</code> object
		''' measures text in a transformed render context. </summary>
		''' <returns>  <code>true</code> if this <code>FontRenderContext</code>
		'''          object has a non-identity AffineTransform attribute.
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.awt.font.FontRenderContext#getTransform
		''' @since   1.6 </seealso>
		Public Overridable Property transformed As Boolean
			Get
				If Not defaulting Then
					Return tx IsNot Nothing
				Else
					Return Not transform.identity
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the integer type of the affine transform for this
		''' <code>FontRenderContext</code> as specified by
		''' <seealso cref="java.awt.geom.AffineTransform#getType()"/> </summary>
		''' <returns> the type of the transform. </returns>
		''' <seealso cref= AffineTransform
		''' @since 1.6 </seealso>
		Public Overridable Property transformType As Integer
			Get
				If Not defaulting Then
					If tx Is Nothing Then
						Return java.awt.geom.AffineTransform.TYPE_IDENTITY
					Else
						Return tx.type
					End If
				Else
					Return transform.type
				End If
			End Get
		End Property

		''' <summary>
		'''   Gets the transform that is used to scale typographical points
		'''   to pixels in this <code>FontRenderContext</code>. </summary>
		'''   <returns> the <code>AffineTransform</code> of this
		'''    <code>FontRenderContext</code>. </returns>
		'''   <seealso cref= AffineTransform </seealso>
		Public Overridable Property transform As java.awt.geom.AffineTransform
			Get
				Return If(tx Is Nothing, New java.awt.geom.AffineTransform, New java.awt.geom.AffineTransform(tx))
			End Get
		End Property

		''' <summary>
		''' Returns a boolean which indicates whether or not some form of
		''' antialiasing is specified by this <code>FontRenderContext</code>.
		''' Call <seealso cref="#getAntiAliasingHint() getAntiAliasingHint()"/>
		''' for the specific rendering hint value. </summary>
		'''   <returns>    <code>true</code>, if text is anti-aliased in this
		'''   <code>FontRenderContext</code>; <code>false</code> otherwise. </returns>
		'''   <seealso cref=        java.awt.RenderingHints#KEY_TEXT_ANTIALIASING </seealso>
		'''   <seealso cref= #FontRenderContext(AffineTransform,boolean,boolean) </seealso>
		'''   <seealso cref= #FontRenderContext(AffineTransform,Object,Object) </seealso>
		Public Overridable Property antiAliased As Boolean
			Get
				Return Not(aaHintValue Is VALUE_TEXT_ANTIALIAS_OFF OrElse aaHintValue Is VALUE_TEXT_ANTIALIAS_DEFAULT)
			End Get
		End Property

		''' <summary>
		''' Returns a boolean which whether text fractional metrics mode
		''' is used in this <code>FontRenderContext</code>.
		''' Call <seealso cref="#getFractionalMetricsHint() getFractionalMetricsHint()"/>
		''' to obtain the corresponding rendering hint value. </summary>
		'''   <returns>    <code>true</code>, if layout should be performed with
		'''   fractional metrics; <code>false</code> otherwise.
		'''               in this <code>FontRenderContext</code>. </returns>
		'''   <seealso cref= java.awt.RenderingHints#KEY_FRACTIONALMETRICS </seealso>
		'''   <seealso cref= #FontRenderContext(AffineTransform,boolean,boolean) </seealso>
		'''   <seealso cref= #FontRenderContext(AffineTransform,Object,Object) </seealso>
		Public Overridable Function usesFractionalMetrics() As Boolean
			Return Not(fmHintValue Is VALUE_FRACTIONALMETRICS_OFF OrElse fmHintValue Is VALUE_FRACTIONALMETRICS_DEFAULT)
		End Function

        ''' <summary>
        ''' Return the text anti-aliasing rendering mode hint used in this
        ''' <code>FontRenderContext</code>.
        ''' This will be one of the text antialiasing rendering hint values
        ''' defined in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>. </summary>
        ''' <returns>  text anti-aliasing rendering mode hint used in this
        ''' <code>FontRenderContext</code>.
        ''' @since 1.6 </returns>
        Public Overridable ReadOnly Property antiAliasingHint As Object
            Get
                If defaulting Then
                    If antiAliased Then
                        Return VALUE_TEXT_ANTIALIAS_ON
                    Else
                        Return VALUE_TEXT_ANTIALIAS_OFF
                    End If
                End If
                Return aaHintValue
            End Get
        End Property

        ''' <summary>
        ''' Return the text fractional metrics rendering mode hint used in this
        ''' <code>FontRenderContext</code>.
        ''' This will be one of the text fractional metrics rendering hint values
        ''' defined in <seealso cref="java.awt.RenderingHints java.awt.RenderingHints"/>. </summary>
        ''' <returns> the text fractional metrics rendering mode hint used in this
        ''' <code>FontRenderContext</code>.
        ''' @since 1.6 </returns>
        Public Overridable ReadOnly Property fractionalMetricsHint As Object
            Get
                If defaulting Then
                    If usesFractionalMetrics() Then
                        Return VALUE_FRACTIONALMETRICS_ON
                    Else
                        Return VALUE_FRACTIONALMETRICS_OFF
                    End If
                End If
                Return fmHintValue
            End Get
        End Property

        ''' <summary>
        ''' Return true if obj is an instance of FontRenderContext and has the same
        ''' transform, antialiasing, and fractional metrics values as this. </summary>
        ''' <param name="obj"> the object to test for equality </param>
        ''' <returns> <code>true</code> if the specified object is equal to
        '''         this <code>FontRenderContext</code>; <code>false</code>
        '''         otherwise. </returns>
        Public Overrides Function Equals(  obj As Object) As Boolean
            Try
                Return Equals(CType(obj, FontRenderContext))
            Catch e As classCastException
                Return False
			End Try
		End Function

		''' <summary>
		''' Return true if rhs has the same transform, antialiasing,
		''' and fractional metrics values as this. </summary>
		''' <param name="rhs"> the <code>FontRenderContext</code> to test for equality </param>
		''' <returns> <code>true</code> if <code>rhs</code> is equal to
		'''         this <code>FontRenderContext</code>; <code>false</code>
		'''         otherwise.
		''' @since 1.4 </returns>
		Public Overrides Function Equals(  rhs As FontRenderContext) As Boolean
			If Me Is rhs Then Return True
			If rhs Is Nothing Then Return False

			' if neither instance is a subclass, reference values directly. 
			If (Not rhs.defaulting) AndAlso (Not defaulting) Then
				If rhs.aaHintValue Is aaHintValue AndAlso rhs.fmHintValue Is fmHintValue Then Return If(tx Is Nothing, rhs.tx Is Nothing, tx.Equals(rhs.tx))
				Return False
			Else
				Return rhs.antiAliasingHint Is antiAliasingHint AndAlso rhs.fractionalMetricsHint Is fractionalMetricsHint AndAlso rhs.transform.Equals(transform)
			End If
		End Function

		''' <summary>
		''' Return a hashcode for this FontRenderContext.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = If(tx Is Nothing, 0, tx.GetHashCode())
	'         SunHints value objects have identity hashcode, so we can rely on
	'         * this to ensure that two equal FRC's have the same hashcode.
	'         
			If defaulting Then
				hash += antiAliasingHint.GetHashCode()
				hash += fractionalMetricsHint.GetHashCode()
			Else
				hash += aaHintValue.GetHashCode()
				hash += fmHintValue.GetHashCode()
			End If
			Return hash
		End Function
	End Class

End Namespace