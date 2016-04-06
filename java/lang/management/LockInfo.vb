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
	''' Information about a <em>lock</em>.  A lock can be a built-in object monitor,
	''' an <em>ownable synchronizer</em>, or the <seealso cref="Condition Condition"/>
	''' object associated with synchronizers.
	''' <p>
	''' <a name="OwnableSynchronizer">An ownable synchronizer</a> is
	''' a synchronizer that may be exclusively owned by a thread and uses
	''' <seealso cref="AbstractOwnableSynchronizer AbstractOwnableSynchronizer"/>
	''' (or its subclass) to implement its synchronization property.
	''' <seealso cref="ReentrantLock ReentrantLock"/> and
	''' <seealso cref="ReentrantReadWriteLock ReentrantReadWriteLock"/> are
	''' two examples of ownable synchronizers provided by the platform.
	''' 
	''' <h3><a name="MappedType">MXBean Mapping</a></h3>
	''' <tt>LockInfo</tt> is mapped to a <seealso cref="CompositeData CompositeData"/>
	''' as specified in the <seealso cref="#from from"/> method.
	''' </summary>
	''' <seealso cref= java.util.concurrent.locks.AbstractOwnableSynchronizer </seealso>
	''' <seealso cref= java.util.concurrent.locks.Condition
	''' 
	''' @author  Mandy Chung
	''' @since   1.6 </seealso>

	Public Class LockInfo

		Private className As String
		Private identityHashCode As Integer

		''' <summary>
		''' Constructs a <tt>LockInfo</tt> object.
		''' </summary>
		''' <param name="className"> the fully qualified name of the class of the lock object. </param>
		''' <param name="identityHashCode"> the {@link System#identityHashCode
		'''                         identity hash code} of the lock object. </param>
		Public Sub New(  className As String,   identityHashCode As Integer)
			If className Is Nothing Then Throw New NullPointerException("Parameter className cannot be null")
			Me.className = className
			Me.identityHashCode = identityHashCode
		End Sub

		''' <summary>
		''' package-private constructors
		''' </summary>
		Friend Sub New(  lock As Object)
			Me.className = lock.GetType().name
			Me.identityHashCode = System.identityHashCode(lock)
		End Sub

		''' <summary>
		''' Returns the fully qualified name of the class of the lock object.
		''' </summary>
		''' <returns> the fully qualified name of the class of the lock object. </returns>
		Public Overridable Property className As String
			Get
				Return className
			End Get
		End Property

		''' <summary>
		''' Returns the identity hash code of the lock object
		''' returned from the <seealso cref="System#identityHashCode"/> method.
		''' </summary>
		''' <returns> the identity hash code of the lock object. </returns>
		Public Overridable Property identityHashCode As Integer
			Get
				Return identityHashCode
			End Get
		End Property

		''' <summary>
		''' Returns a {@code LockInfo} object represented by the
		''' given {@code CompositeData}.
		''' The given {@code CompositeData} must contain the following attributes:
		''' <blockquote>
		''' <table border summary="The attributes and the types the given CompositeData contains">
		''' <tr>
		'''   <th align=left>Attribute Name</th>
		'''   <th align=left>Type</th>
		''' </tr>
		''' <tr>
		'''   <td>className</td>
		'''   <td><tt>java.lang.String</tt></td>
		''' </tr>
		''' <tr>
		'''   <td>identityHashCode</td>
		'''   <td><tt>java.lang.Integer</tt></td>
		''' </tr>
		''' </table>
		''' </blockquote>
		''' </summary>
		''' <param name="cd"> {@code CompositeData} representing a {@code LockInfo}
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code cd} does not
		'''   represent a {@code LockInfo} with the attributes described
		'''   above. </exception>
		''' <returns> a {@code LockInfo} object represented
		'''         by {@code cd} if {@code cd} is not {@code null};
		'''         {@code null} otherwise.
		''' 
		''' @since 1.8 </returns>
		Public Shared Function [from](  cd As javax.management.openmbean.CompositeData) As LockInfo
			If cd Is Nothing Then Return Nothing

			If TypeOf cd Is sun.management.LockInfoCompositeData Then
				Return CType(cd, sun.management.LockInfoCompositeData).lockInfo
			Else
				Return sun.management.LockInfoCompositeData.toLockInfo(cd)
			End If
		End Function

		''' <summary>
		''' Returns a string representation of a lock.  The returned
		''' string representation consists of the name of the class of the
		''' lock object, the at-sign character `@', and the unsigned
		''' hexadecimal representation of the <em>identity</em> hash code
		''' of the object.  This method returns a string equals to the value of:
		''' <blockquote>
		''' <pre>
		''' lock.getClass().getName() + '@' +  java.lang.[Integer].toHexString(System.identityHashCode(lock))
		''' </pre></blockquote>
		''' where <tt>lock</tt> is the lock object.
		''' </summary>
		''' <returns> the string representation of a lock. </returns>
		Public Overrides Function ToString() As String
			Return className + AscW("@"c) +  java.lang.[Integer].toHexString(identityHashCode)
		End Function
	End Class

End Namespace