'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Information about an object monitor lock.  An object monitor is locked
	''' when entering a synchronization block or method on that object.
	''' 
	''' <h3>MXBean Mapping</h3>
	''' <tt>MonitorInfo</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	''' with attributes as specified in
	''' the <seealso cref="#from from"/> method.
	''' 
	''' @author  Mandy Chung
	''' @since   1.6
	''' </summary>
	Public Class MonitorInfo
		Inherits LockInfo

		Private stackDepth As Integer
		Private stackFrame As StackTraceElement

		''' <summary>
		''' Construct a <tt>MonitorInfo</tt> object.
		''' </summary>
		''' <param name="className"> the fully qualified name of the class of the lock object. </param>
		''' <param name="identityHashCode"> the {@link System#identityHashCode
		'''                         identity hash code} of the lock object. </param>
		''' <param name="stackDepth"> the depth in the stack trace where the object monitor
		'''                   was locked. </param>
		''' <param name="stackFrame"> the stack frame that locked the object monitor. </param>
		''' <exception cref="IllegalArgumentException"> if
		'''    <tt>stackDepth</tt> &ge; 0 but <tt>stackFrame</tt> is <tt>null</tt>,
		'''    or <tt>stackDepth</tt> &lt; 0 but <tt>stackFrame</tt> is not
		'''       <tt>null</tt>. </exception>
		Public Sub New(  className As String,   identityHashCode As Integer,   stackDepth As Integer,   stackFrame As StackTraceElement)
			MyBase.New(className, identityHashCode)
			If stackDepth >= 0 AndAlso stackFrame Is Nothing Then Throw New IllegalArgumentException("Parameter stackDepth is " & stackDepth & " but stackFrame is null")
			If stackDepth < 0 AndAlso stackFrame IsNot Nothing Then Throw New IllegalArgumentException("Parameter stackDepth is " & stackDepth & " but stackFrame is not null")
			Me.stackDepth = stackDepth
			Me.stackFrame = stackFrame
		End Sub

		''' <summary>
		''' Returns the depth in the stack trace where the object monitor
		''' was locked.  The depth is the index to the <tt>StackTraceElement</tt>
		''' array returned in the <seealso cref="ThreadInfo#getStackTrace"/> method.
		''' </summary>
		''' <returns> the depth in the stack trace where the object monitor
		'''         was locked, or a negative number if not available. </returns>
		Public Overridable Property lockedStackDepth As Integer
			Get
				Return stackDepth
			End Get
		End Property

		''' <summary>
		''' Returns the stack frame that locked the object monitor.
		''' </summary>
		''' <returns> <tt>StackTraceElement</tt> that locked the object monitor,
		'''         or <tt>null</tt> if not available. </returns>
		Public Overridable Property lockedStackFrame As StackTraceElement
			Get
				Return stackFrame
			End Get
		End Property

		''' <summary>
		''' Returns a <tt>MonitorInfo</tt> object represented by the
		''' given <tt>CompositeData</tt>.
		''' The given <tt>CompositeData</tt> must contain the following attributes
		''' as well as the attributes specified in the
		''' <a href="LockInfo.html#MappedType">
		''' mapped type</a> for the <seealso cref="LockInfo"/> class:
		''' <blockquote>
		''' <table border summary="The attributes and their types the given CompositeData contains">
		''' <tr>
		'''   <th align=left>Attribute Name</th>
		'''   <th align=left>Type</th>
		''' </tr>
		''' <tr>
		'''   <td>lockedStackFrame</td>
		'''   <td><tt>CompositeData as specified in the
		'''       <a href="ThreadInfo.html#StackTrace">stackTrace</a>
		'''       attribute defined in the {@link ThreadInfo#from
		'''       ThreadInfo.from} method.
		'''       </tt></td>
		''' </tr>
		''' <tr>
		'''   <td>lockedStackDepth</td>
		'''   <td><tt>java.lang.Integer</tt></td>
		''' </tr>
		''' </table>
		''' </blockquote>
		''' </summary>
		''' <param name="cd"> <tt>CompositeData</tt> representing a <tt>MonitorInfo</tt>
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <tt>cd</tt> does not
		'''   represent a <tt>MonitorInfo</tt> with the attributes described
		'''   above.
		''' </exception>
		''' <returns> a <tt>MonitorInfo</tt> object represented
		'''         by <tt>cd</tt> if <tt>cd</tt> is not <tt>null</tt>;
		'''         <tt>null</tt> otherwise. </returns>
		Public Shared Function [from](  cd As javax.management.openmbean.CompositeData) As MonitorInfo
			If cd Is Nothing Then Return Nothing

			If TypeOf cd Is sun.management.MonitorInfoCompositeData Then
				Return CType(cd, sun.management.MonitorInfoCompositeData).monitorInfo
			Else
				sun.management.MonitorInfoCompositeData.validateCompositeData(cd)
				Dim className_Renamed As String = sun.management.MonitorInfoCompositeData.getClassName(cd)
				Dim identityHashCode_Renamed As Integer = sun.management.MonitorInfoCompositeData.getIdentityHashCode(cd)
				Dim stackDepth As Integer = sun.management.MonitorInfoCompositeData.getLockedStackDepth(cd)
				Dim stackFrame As StackTraceElement = sun.management.MonitorInfoCompositeData.getLockedStackFrame(cd)
				Return New MonitorInfo(className_Renamed, identityHashCode_Renamed, stackDepth, stackFrame)
			End If
		End Function

	End Class

End Namespace