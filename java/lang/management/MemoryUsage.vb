'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.management


	''' <summary>
	''' A <tt>MemoryUsage</tt> object represents a snapshot of memory usage.
	''' Instances of the <tt>MemoryUsage</tt> class are usually constructed
	''' by methods that are used to obtain memory usage
	''' information about individual memory pool of the Java virtual machine or
	''' the heap or non-heap memory of the Java virtual machine as a whole.
	''' 
	''' <p> A <tt>MemoryUsage</tt> object contains four values:
	''' <table summary="Describes the MemoryUsage object content">
	''' <tr>
	''' <td valign=top> <tt>init</tt> </td>
	''' <td valign=top> represents the initial amount of memory (in bytes) that
	'''      the Java virtual machine requests from the operating system
	'''      for memory management during startup.  The Java virtual machine
	'''      may request additional memory from the operating system and
	'''      may also release memory to the system over time.
	'''      The value of <tt>init</tt> may be undefined.
	''' </td>
	''' </tr>
	''' <tr>
	''' <td valign=top> <tt>used</tt> </td>
	''' <td valign=top> represents the amount of memory currently used (in bytes).
	''' </td>
	''' </tr>
	''' <tr>
	''' <td valign=top> <tt>committed</tt> </td>
	''' <td valign=top> represents the amount of memory (in bytes) that is
	'''      guaranteed to be available for use by the Java virtual machine.
	'''      The amount of committed memory may change over time (increase
	'''      or decrease).  The Java virtual machine may release memory to
	'''      the system and <tt>committed</tt> could be less than <tt>init</tt>.
	'''      <tt>committed</tt> will always be greater than
	'''      or equal to <tt>used</tt>.
	''' </td>
	''' </tr>
	''' <tr>
	''' <td valign=top> <tt>max</tt> </td>
	''' <td valign=top> represents the maximum amount of memory (in bytes)
	'''      that can be used for memory management. Its value may be undefined.
	'''      The maximum amount of memory may change over time if defined.
	'''      The amount of used and committed memory will always be less than
	'''      or equal to <tt>max</tt> if <tt>max</tt> is defined.
	'''      A memory allocation may fail if it attempts to increase the
	'''      used memory such that <tt>used &gt; committed</tt> even
	'''      if <tt>used &lt;= max</tt> would still be true (for example,
	'''      when the system is low on virtual memory).
	''' </td>
	''' </tr>
	''' </table>
	''' 
	''' Below is a picture showing an example of a memory pool:
	''' 
	''' <pre>
	'''        +----------------------------------------------+
	'''        +////////////////           |                  +
	'''        +////////////////           |                  +
	'''        +----------------------------------------------+
	''' 
	'''        |--------|
	'''           init
	'''        |---------------|
	'''               used
	'''        |---------------------------|
	'''                  committed
	'''        |----------------------------------------------|
	'''                            max
	''' </pre>
	''' 
	''' <h3>MXBean Mapping</h3>
	''' <tt>MemoryUsage</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	''' with attributes as specified in the <seealso cref="#from from"/> method.
	''' 
	''' @author   Mandy Chung
	''' @since   1.5
	''' </summary>
	Public Class MemoryUsage
		Private ReadOnly init As Long
		Private ReadOnly used As Long
		Private ReadOnly committed As Long
		Private ReadOnly max As Long

		''' <summary>
		''' Constructs a <tt>MemoryUsage</tt> object.
		''' </summary>
		''' <param name="init">      the initial amount of memory in bytes that
		'''                  the Java virtual machine allocates;
		'''                  or <tt>-1</tt> if undefined. </param>
		''' <param name="used">      the amount of used memory in bytes. </param>
		''' <param name="committed"> the amount of committed memory in bytes. </param>
		''' <param name="max">       the maximum amount of memory in bytes that
		'''                  can be used; or <tt>-1</tt> if undefined.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <ul>
		''' <li> the value of <tt>init</tt> or <tt>max</tt> is negative
		'''      but not <tt>-1</tt>; or</li>
		''' <li> the value of <tt>used</tt> or <tt>committed</tt> is negative;
		'''      or</li>
		''' <li> <tt>used</tt> is greater than the value of <tt>committed</tt>;
		'''      or</li>
		''' <li> <tt>committed</tt> is greater than the value of <tt>max</tt>
		'''      <tt>max</tt> if defined.</li>
		''' </ul> </exception>
		Public Sub New(ByVal init As Long, ByVal used As Long, ByVal committed As Long, ByVal max As Long)
			If init < -1 Then Throw New IllegalArgumentException("init parameter = " & init & " is negative but not -1.")
			If max < -1 Then Throw New IllegalArgumentException("max parameter = " & max & " is negative but not -1.")
			If used < 0 Then Throw New IllegalArgumentException("used parameter = " & used & " is negative.")
			If committed < 0 Then Throw New IllegalArgumentException("committed parameter = " & committed & " is negative.")
			If used > committed Then Throw New IllegalArgumentException("used = " & used & " should be <= committed = " & committed)
			If max >= 0 AndAlso committed > max Then Throw New IllegalArgumentException("committed = " & committed & " should be < max = " & max)

			Me.init = init
			Me.used = used
			Me.committed = committed
			Me.max = max
		End Sub

		''' <summary>
		''' Constructs a <tt>MemoryUsage</tt> object from a
		''' <seealso cref="CompositeData CompositeData"/>.
		''' </summary>
		Private Sub New(ByVal cd As javax.management.openmbean.CompositeData)
			' validate the input composite data
			sun.management.MemoryUsageCompositeData.validateCompositeData(cd)

			Me.init = sun.management.MemoryUsageCompositeData.getInit(cd)
			Me.used = sun.management.MemoryUsageCompositeData.getUsed(cd)
			Me.committed = sun.management.MemoryUsageCompositeData.getCommitted(cd)
			Me.max = sun.management.MemoryUsageCompositeData.getMax(cd)
		End Sub

		''' <summary>
		''' Returns the amount of memory in bytes that the Java virtual machine
		''' initially requests from the operating system for memory management.
		''' This method returns <tt>-1</tt> if the initial memory size is undefined.
		''' </summary>
		''' <returns> the initial size of memory in bytes;
		''' <tt>-1</tt> if undefined. </returns>
		Public Overridable Property init As Long
			Get
				Return init
			End Get
		End Property

		''' <summary>
		''' Returns the amount of used memory in bytes.
		''' </summary>
		''' <returns> the amount of used memory in bytes.
		'''  </returns>
		Public Overridable Property used As Long
			Get
				Return used
			End Get
		End Property

		''' <summary>
		''' Returns the amount of memory in bytes that is committed for
		''' the Java virtual machine to use.  This amount of memory is
		''' guaranteed for the Java virtual machine to use.
		''' </summary>
		''' <returns> the amount of committed memory in bytes.
		'''  </returns>
		Public Overridable Property committed As Long
			Get
				Return committed
			End Get
		End Property

		''' <summary>
		''' Returns the maximum amount of memory in bytes that can be
		''' used for memory management.  This method returns <tt>-1</tt>
		''' if the maximum memory size is undefined.
		''' 
		''' <p> This amount of memory is not guaranteed to be available
		''' for memory management if it is greater than the amount of
		''' committed memory.  The Java virtual machine may fail to allocate
		''' memory even if the amount of used memory does not exceed this
		''' maximum size.
		''' </summary>
		''' <returns> the maximum amount of memory in bytes;
		''' <tt>-1</tt> if undefined. </returns>
		Public Overridable Property max As Long
			Get
				Return max
			End Get
		End Property

		''' <summary>
		''' Returns a descriptive representation of this memory usage.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuffer
			buf.append("init = " & init & "(" & (init >> 10) & "K) ")
			buf.append("used = " & used & "(" & (used >> 10) & "K) ")
			buf.append("committed = " & committed & "(" & (committed >> 10) & "K) ")
			buf.append("max = " & max & "(" & (max >> 10) & "K)")
			Return buf.ToString()
		End Function

		''' <summary>
		''' Returns a <tt>MemoryUsage</tt> object represented by the
		''' given <tt>CompositeData</tt>. The given <tt>CompositeData</tt>
		''' must contain the following attributes:
		''' 
		''' <blockquote>
		''' <table border summary="The attributes and the types the given CompositeData contains">
		''' <tr>
		'''   <th align=left>Attribute Name</th>
		'''   <th align=left>Type</th>
		''' </tr>
		''' <tr>
		'''   <td>init</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>used</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>committed</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>max</td>
		'''   <td><tt>java.lang.Long</tt></td>
		''' </tr>
		''' </table>
		''' </blockquote>
		''' </summary>
		''' <param name="cd"> <tt>CompositeData</tt> representing a <tt>MemoryUsage</tt>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		'''   represent a <tt>MemoryUsage</tt> with the attributes described
		'''   above.
		''' </exception>
		''' <returns> a <tt>MemoryUsage</tt> object represented by <tt>cd</tt>
		'''         if <tt>cd</tt> is not <tt>null</tt>;
		'''         <tt>null</tt> otherwise. </returns>
		Public Shared Function [from](ByVal cd As javax.management.openmbean.CompositeData) As MemoryUsage
			If cd Is Nothing Then Return Nothing

			If TypeOf cd Is sun.management.MemoryUsageCompositeData Then
				Return CType(cd, sun.management.MemoryUsageCompositeData).memoryUsage
			Else
				Return New MemoryUsage(cd)
			End If

		End Function
	End Class

End Namespace