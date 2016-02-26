Imports System
Imports System.Text

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


Namespace javax.print.attribute


	''' <summary>
	''' Class ResolutionSyntax is an abstract base class providing the common
	''' implementation of all attributes denoting a printer resolution.
	''' <P>
	''' A resolution attribute's value consists of two items, the cross feed
	''' direction resolution and the feed direction resolution. A resolution
	''' attribute may be constructed by supplying the two values and indicating the
	''' units in which the values are measured. Methods are provided to return a
	''' resolution attribute's values, indicating the units in which the values are
	''' to be returned. The two most common resolution units are dots per inch (dpi)
	''' and dots per centimeter (dpcm), and exported constants {@link #DPI
	''' DPI} and <seealso cref="#DPCM DPCM"/> are provided for
	''' indicating those units.
	''' <P>
	''' Once constructed, a resolution attribute's value is immutable.
	''' <P>
	''' <B>Design</B>
	''' <P>
	''' A resolution attribute's cross feed direction resolution and feed direction
	''' resolution values are stored internally using units of dots per 100 inches
	''' (dphi). Storing the values in dphi rather than, say, metric units allows
	''' precise integer arithmetic conversions between dpi and dphi and between dpcm
	''' and dphi: 1 dpi = 100 dphi, 1 dpcm = 254 dphi. Thus, the values can be stored
	''' into and retrieved back from a resolution attribute in either units with no
	''' loss of precision. This would not be guaranteed if a floating point
	''' representation were used. However, roundoff error will in general occur if a
	''' resolution attribute's values are created in one units and retrieved in
	''' different units; for example, 600 dpi will be rounded to 236 dpcm, whereas
	''' the true value (to five figures) is 236.22 dpcm.
	''' <P>
	''' Storing the values internally in common units of dphi lets two resolution
	''' attributes be compared without regard to the units in which they were
	''' created; for example, 300 dpcm will compare equal to 762 dpi, as they both
	''' are stored as 76200 dphi. In particular, a lookup service can
	''' match resolution attributes based on equality of their serialized
	''' representations regardless of the units in which they were created. Again,
	''' using integers for internal storage allows precise equality comparisons to be
	''' done, which would not be guaranteed if a floating point representation were
	''' used.
	''' <P>
	''' The exported constant <seealso cref="#DPI DPI"/> is actually the
	''' conversion factor by which to multiply a value in dpi to get the value in
	''' dphi. Likewise, the exported constant <seealso cref="#DPCM DPCM"/> is the
	''' conversion factor by which to multiply a value in dpcm to get the value in
	''' dphi. A client can specify a resolution value in units other than dpi or dpcm
	''' by supplying its own conversion factor. However, since the internal units of
	''' dphi was chosen with supporting only the external units of dpi and dpcm in
	''' mind, there is no guarantee that the conversion factor for the client's units
	''' will be an exact integer. If the conversion factor isn't an exact integer,
	''' resolution values in the client's units won't be stored precisely.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class ResolutionSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = 2706743076526672017L

		''' <summary>
		''' Cross feed direction resolution in units of dots per 100 inches (dphi).
		''' @serial
		''' </summary>
		Private crossFeedResolution As Integer

		''' <summary>
		''' Feed direction resolution in units of dots per 100 inches (dphi).
		''' @serial
		''' </summary>
		Private feedResolution As Integer

		''' <summary>
		''' Value to indicate units of dots per inch (dpi). It is actually the
		''' conversion factor by which to multiply dpi to yield dphi (100).
		''' </summary>
		Public Const DPI As Integer = 100

		''' <summary>
		''' Value to indicate units of dots per centimeter (dpcm). It is actually
		''' the conversion factor by which to multiply dpcm to yield dphi (254).
		''' </summary>
		Public Const DPCM As Integer = 254


		''' <summary>
		''' Construct a new resolution attribute from the given items.
		''' </summary>
		''' <param name="crossFeedResolution">
		'''     Cross feed direction resolution. </param>
		''' <param name="feedResolution">
		'''     Feed direction resolution. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#DPI DPI"/> or
		''' <seealso cref="   #DPCM DPCM"/>.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code crossFeedResolution < 1}
		'''     or {@code feedResolution < 1} or {@code units < 1}. </exception>
		Public Sub New(ByVal crossFeedResolution As Integer, ByVal feedResolution As Integer, ByVal units As Integer)

			If crossFeedResolution < 1 Then Throw New System.ArgumentException("crossFeedResolution is < 1")
			If feedResolution < 1 Then Throw New System.ArgumentException("feedResolution is < 1")
			If units < 1 Then Throw New System.ArgumentException("units is < 1")

			Me.crossFeedResolution = crossFeedResolution * units
			Me.feedResolution = feedResolution * units
		End Sub

		''' <summary>
		''' Convert a value from dphi to some other units. The result is rounded to
		''' the nearest integer.
		''' </summary>
		''' <param name="dphi">
		'''     Value (dphi) to convert. </param>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#DPI <CODE>DPI</CODE>"/> or
		''' <seealso cref="    #DPCM <CODE>DPCM</CODE>"/>.
		''' </param>
		''' <returns>  The value of <CODE>dphi</CODE> converted to the desired units.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>units</CODE> < 1. </exception>
		Private Shared Function convertFromDphi(ByVal dphi As Integer, ByVal units As Integer) As Integer
			If units < 1 Then Throw New System.ArgumentException(": units is < 1")
			Dim round As Integer = units \ 2
			Return (dphi + round) \ units
		End Function

		''' <summary>
		''' Get this resolution attribute's resolution values in the given units.
		''' The values are rounded to the nearest integer.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#DPI DPI"/> or
		''' <seealso cref="  #DPCM DPCM"/>.
		''' </param>
		''' <returns>  A two-element array with the cross feed direction resolution
		'''          at index 0 and the feed direction resolution at index 1.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overridable Function getResolution(ByVal units As Integer) As Integer()
			Return New Integer() { getCrossFeedResolution(units), getFeedResolution(units) }
		End Function

		''' <summary>
		''' Returns this resolution attribute's cross feed direction resolution in
		''' the given units. The value is rounded to the nearest integer.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#DPI DPI"/> or
		''' <seealso cref=" #DPCM DPCM"/>.
		''' </param>
		''' <returns>  Cross feed direction resolution.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overridable Function getCrossFeedResolution(ByVal units As Integer) As Integer
			Return convertFromDphi(crossFeedResolution, units)
		End Function

		''' <summary>
		''' Returns this resolution attribute's feed direction resolution in the
		''' given units. The value is rounded to the nearest integer.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#DPI DPI"/> or {@link
		'''     #DPCM DPCM}.
		''' </param>
		''' <returns>  Feed direction resolution.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overridable Function getFeedResolution(ByVal units As Integer) As Integer
			Return convertFromDphi(feedResolution, units)
		End Function

		''' <summary>
		''' Returns a string version of this resolution attribute in the given units.
		''' The string takes the form <CODE>"<I>C</I>x<I>F</I> <I>U</I>"</CODE>,
		''' where <I>C</I> is the cross feed direction resolution, <I>F</I> is the
		''' feed direction resolution, and <I>U</I> is the units name. The values are
		''' rounded to the nearest integer.
		''' </summary>
		''' <param name="units">
		'''     Unit conversion factor, e.g. <seealso cref="#DPI CODE>DPI"/> or {@link
		'''     #DPCM DPCM}. </param>
		''' <param name="unitsName">
		'''     Units name string, e.g. <CODE>"dpi"</CODE> or <CODE>"dpcm"</CODE>. If
		'''     null, no units name is appended to the result.
		''' </param>
		''' <returns>  String version of this resolution attribute.
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if {@code units < 1}. </exception>
		Public Overrides Function ToString(ByVal units As Integer, ByVal unitsName As String) As String
			Dim result As New StringBuilder
			result.Append(getCrossFeedResolution(units))
			result.Append("x"c)
			result.Append(getFeedResolution(units))
			If unitsName IsNot Nothing Then
				result.Append(" "c)
				result.Append(unitsName)
			End If
			Return result.ToString()
		End Function


		''' <summary>
		''' Determine whether this resolution attribute's value is less than or
		''' equal to the given resolution attribute's value. This is true if all
		''' of the following conditions are true:
		''' <UL>
		''' <LI>
		''' This attribute's cross feed direction resolution is less than or equal to
		''' the <CODE>other</CODE> attribute's cross feed direction resolution.
		''' <LI>
		''' This attribute's feed direction resolution is less than or equal to the
		''' <CODE>other</CODE> attribute's feed direction resolution.
		''' </UL>
		''' </summary>
		''' <param name="other">  Resolution attribute to compare with.
		''' </param>
		''' <returns>  True if this resolution attribute is less than or equal to the
		'''          <CODE>other</CODE> resolution attribute, false otherwise.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>other</CODE> is null. </exception>
		Public Overridable Function lessThanOrEquals(ByVal other As ResolutionSyntax) As Boolean
			Return (Me.crossFeedResolution <= other.crossFeedResolution AndAlso Me.feedResolution <= other.feedResolution)
		End Function


		''' <summary>
		''' Returns whether this resolution attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class ResolutionSyntax.
		''' <LI>
		''' This attribute's cross feed direction resolution is equal to
		''' <CODE>object</CODE>'s cross feed direction resolution.
		''' <LI>
		''' This attribute's feed direction resolution is equal to
		''' <CODE>object</CODE>'s feed direction resolution.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this resolution
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean

			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is ResolutionSyntax AndAlso Me.crossFeedResolution = CType([object], ResolutionSyntax).crossFeedResolution AndAlso Me.feedResolution = CType([object], ResolutionSyntax).feedResolution)
		End Function

		''' <summary>
		''' Returns a hash code value for this resolution attribute.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return (((crossFeedResolution And &HFFFF)) Or ((feedResolution And &HFFFF) << 16))
		End Function

		''' <summary>
		''' Returns a string version of this resolution attribute. The string takes
		''' the form <CODE>"<I>C</I>x<I>F</I> dphi"</CODE>, where <I>C</I> is the
		''' cross feed direction resolution and <I>F</I> is the feed direction
		''' resolution. The values are reported in the internal units of dphi.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim result As New StringBuilder
			result.Append(crossFeedResolution)
			result.Append("x"c)
			result.Append(feedResolution)
			result.Append(" dphi")
			Return result.ToString()
		End Function


		''' <summary>
		''' Returns this resolution attribute's cross feed direction resolution in
		''' units of dphi. (For use in a subclass.)
		''' </summary>
		''' <returns>  Cross feed direction resolution. </returns>
		Protected Friend Overridable Property crossFeedResolutionDphi As Integer
			Get
				Return crossFeedResolution
			End Get
		End Property

		''' <summary>
		''' Returns this resolution attribute's feed direction resolution in units
		''' of dphi. (For use in a subclass.)
		''' </summary>
		''' <returns>  Feed direction resolution. </returns>
		Protected Friend Overridable Property feedResolutionDphi As Integer
			Get
				Return feedResolution
			End Get
		End Property

	End Class

End Namespace