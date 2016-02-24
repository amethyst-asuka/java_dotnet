Imports System
Imports System.Threading

'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.rmi.server


	''' <summary>
	''' A <code>UID</code> represents an identifier that is unique over time
	''' with respect to the host it is generated on, or one of 2<sup>16</sup>
	''' "well-known" identifiers.
	''' 
	''' <p>The <seealso cref="#UID()"/> constructor can be used to generate an
	''' identifier that is unique over time with respect to the host it is
	''' generated on.  The <seealso cref="#UID(short)"/> constructor can be used to
	''' create one of 2<sup>16</sup> well-known identifiers.
	''' 
	''' <p>A <code>UID</code> instance contains three primitive values:
	''' <ul>
	''' <li><code>unique</code>, an <code>int</code> that uniquely identifies
	''' the VM that this <code>UID</code> was generated in, with respect to its
	''' host and at the time represented by the <code>time</code> value (an
	''' example implementation of the <code>unique</code> value would be a
	''' process identifier),
	'''  or zero for a well-known <code>UID</code>
	''' <li><code>time</code>, a <code>long</code> equal to a time (as returned
	''' by <seealso cref="System#currentTimeMillis()"/>) at which the VM that this
	''' <code>UID</code> was generated in was alive,
	''' or zero for a well-known <code>UID</code>
	''' <li><code>count</code>, a <code>short</code> to distinguish
	''' <code>UID</code>s generated in the same VM with the same
	''' <code>time</code> value
	''' </ul>
	''' 
	''' <p>An independently generated <code>UID</code> instance is unique
	''' over time with respect to the host it is generated on as long as
	''' the host requires more than one millisecond to reboot and its system
	''' clock is never set backward.  A globally unique identifier can be
	''' constructed by pairing a <code>UID</code> instance with a unique host
	''' identifier, such as an IP address.
	''' 
	''' @author      Ann Wollrath
	''' @author      Peter Jones
	''' @since       JDK1.1
	''' </summary>
	<Serializable> _
	Public NotInheritable Class UID

		Private Shared hostUnique As Integer
		Private Shared hostUniqueSet As Boolean = False

		Private Shared ReadOnly lock As New Object
		Private Shared lastTime As Long = System.currentTimeMillis()
		Private Shared lastCount As Short = Short.MIN_VALUE

		''' <summary>
		''' indicate compatibility with JDK 1.1.x version of class </summary>
		Private Const serialVersionUID As Long = 1086053664494604050L

		''' <summary>
		''' number that uniquely identifies the VM that this <code>UID</code>
		''' was generated in with respect to its host and at the given time
		''' @serial
		''' </summary>
		Private ReadOnly unique As Integer

		''' <summary>
		''' a time (as returned by <seealso cref="System#currentTimeMillis()"/>) at which
		''' the VM that this <code>UID</code> was generated in was alive
		''' @serial
		''' </summary>
		Private ReadOnly time As Long

		''' <summary>
		''' 16-bit number to distinguish <code>UID</code> instances created
		''' in the same VM with the same time value
		''' @serial
		''' </summary>
		Private ReadOnly count As Short

		''' <summary>
		''' Generates a <code>UID</code> that is unique over time with
		''' respect to the host that it was generated on.
		''' </summary>
		Public Sub New()

			SyncLock lock
				If Not hostUniqueSet Then
					hostUnique = (New java.security.SecureRandom).Next()
					hostUniqueSet = True
				End If
				unique = hostUnique
				If lastCount = Short.MaxValue Then
					Dim interrupted As Boolean = Thread.interrupted()
					Dim done As Boolean = False
					Do While Not done
						Dim now As Long = System.currentTimeMillis()
						If now = lastTime Then
							' wait for time to change
							Try
								Thread.Sleep(1)
							Catch e As InterruptedException
								interrupted = True
							End Try
						Else
							' If system time has gone backwards increase
							' original by 1ms to maintain uniqueness
							lastTime = If(now < lastTime, lastTime+1, now)
							lastCount = Short.MinValue
							done = True
						End If
					Loop
					If interrupted Then Thread.CurrentThread.Interrupt()
				End If
				time = lastTime
				count = lastCount
				lastCount += 1
			End SyncLock
		End Sub

		''' <summary>
		''' Creates a "well-known" <code>UID</code>.
		''' 
		''' There are 2<sup>16</sup> possible such well-known ids.
		''' 
		''' <p>A <code>UID</code> created via this constructor will not
		''' clash with any <code>UID</code>s generated via the no-arg
		''' constructor.
		''' </summary>
		''' <param name="num"> number for well-known <code>UID</code> </param>
		Public Sub New(ByVal num As Short)
			unique = 0
			time = 0
			count = num
		End Sub

		''' <summary>
		''' Constructs a <code>UID</code> given data read from a stream.
		''' </summary>
		Private Sub New(ByVal unique As Integer, ByVal time As Long, ByVal count As Short)
			Me.unique = unique
			Me.time = time
			Me.count = count
		End Sub

		''' <summary>
		''' Returns the hash code value for this <code>UID</code>.
		''' </summary>
		''' <returns>  the hash code value for this <code>UID</code> </returns>
		Public Overrides Function GetHashCode() As Integer
			Return CInt(time) + CInt(count)
		End Function

		''' <summary>
		''' Compares the specified object with this <code>UID</code> for
		''' equality.
		''' 
		''' This method returns <code>true</code> if and only if the
		''' specified object is a <code>UID</code> instance with the same
		''' <code>unique</code>, <code>time</code>, and <code>count</code>
		''' values as this one.
		''' </summary>
		''' <param name="obj"> the object to compare this <code>UID</code> to
		''' </param>
		''' <returns>  <code>true</code> if the given object is equivalent to
		''' this one, and <code>false</code> otherwise </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is UID Then
				Dim uid As UID = CType(obj, UID)
				Return (unique = uid.unique AndAlso count = uid.count AndAlso time = uid.time)
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns a string representation of this <code>UID</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>UID</code> </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString(unique,16) & ":" & Convert.ToString(time,16) & ":" & Convert.ToString(count,16)
		End Function

		''' <summary>
		''' Marshals a binary representation of this <code>UID</code> to
		''' a <code>DataOutput</code> instance.
		''' 
		''' <p>Specifically, this method first invokes the given stream's
		''' <seealso cref="DataOutput#writeInt(int)"/> method with this <code>UID</code>'s
		''' <code>unique</code> value, then it invokes the stream's
		''' <seealso cref="DataOutput#writeLong(long)"/> method with this <code>UID</code>'s
		''' <code>time</code> value, and then it invokes the stream's
		''' <seealso cref="DataOutput#writeShort(int)"/> method with this <code>UID</code>'s
		''' <code>count</code> value.
		''' </summary>
		''' <param name="out"> the <code>DataOutput</code> instance to write
		''' this <code>UID</code> to
		''' </param>
		''' <exception cref="IOException"> if an I/O error occurs while performing
		''' this operation </exception>
		Public Sub write(ByVal out As java.io.DataOutput)
			out.writeInt(unique)
			out.writeLong(time)
			out.writeShort(count)
		End Sub

		''' <summary>
		''' Constructs and returns a new <code>UID</code> instance by
		''' unmarshalling a binary representation from an
		''' <code>DataInput</code> instance.
		''' 
		''' <p>Specifically, this method first invokes the given stream's
		''' <seealso cref="DataInput#readInt()"/> method to read a <code>unique</code> value,
		''' then it invoke's the stream's
		''' <seealso cref="DataInput#readLong()"/> method to read a <code>time</code> value,
		''' then it invoke's the stream's
		''' <seealso cref="DataInput#readShort()"/> method to read a <code>count</code> value,
		''' and then it creates and returns a new <code>UID</code> instance
		''' that contains the <code>unique</code>, <code>time</code>, and
		''' <code>count</code> values that were read from the stream.
		''' </summary>
		''' <param name="in"> the <code>DataInput</code> instance to read
		''' <code>UID</code> from
		''' </param>
		''' <returns>  unmarshalled <code>UID</code> instance
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs while performing
		''' this operation </exception>
		Public Shared Function read(ByVal [in] As java.io.DataInput) As UID
			Dim unique As Integer = [in].readInt()
			Dim time As Long = [in].readLong()
			Dim count As Short = [in].readShort()
			Return New UID(unique, time, count)
		End Function
	End Class

End Namespace