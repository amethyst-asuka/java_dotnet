Imports System

'
' * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' An <code>ObjID</code> is used to identify a remote object exported
	''' to an RMI runtime.  When a remote object is exported, it is assigned
	''' an object identifier either implicitly or explicitly, depending on
	''' the API used to export.
	''' 
	''' <p>The <seealso cref="#ObjID()"/> constructor can be used to generate a unique
	''' object identifier.  Such an <code>ObjID</code> is unique over time
	''' with respect to the host it is generated on.
	''' 
	''' The <seealso cref="#ObjID(int)"/> constructor can be used to create a
	''' "well-known" object identifier.  The scope of a well-known
	''' <code>ObjID</code> depends on the RMI runtime it is exported to.
	''' 
	''' <p>An <code>ObjID</code> instance contains an object number (of type
	''' <code>long</code>) and an address space identifier (of type
	''' <seealso cref="UID"/>).  In a unique <code>ObjID</code>, the address space
	''' identifier is unique with respect to a given host over time.  In a
	''' well-known <code>ObjID</code>, the address space identifier is
	''' equivalent to one returned by invoking the <seealso cref="UID#UID(short)"/>
	''' constructor with the value zero.
	''' 
	''' <p>If the system property <code>java.rmi.server.randomIDs</code>
	''' is defined to equal the string <code>"true"</code> (case insensitive),
	''' then the <seealso cref="#ObjID()"/> constructor will use a cryptographically
	''' strong random number generator to choose the object number of the
	''' returned <code>ObjID</code>.
	''' 
	''' @author      Ann Wollrath
	''' @author      Peter Jones
	''' @since       JDK1.1
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ObjID

		''' <summary>
		''' Object number for well-known <code>ObjID</code> of the registry. </summary>
		Public Const REGISTRY_ID As Integer = 0

		''' <summary>
		''' Object number for well-known <code>ObjID</code> of the activator. </summary>
		Public Const ACTIVATOR_ID As Integer = 1

		''' <summary>
		''' Object number for well-known <code>ObjID</code> of
		''' the distributed garbage collector.
		''' </summary>
		Public Const DGC_ID As Integer = 2

		''' <summary>
		''' indicate compatibility with JDK 1.1.x version of class </summary>
		Private Const serialVersionUID As Long = -6386392263968365220L

		Private Shared ReadOnly nextObjNum As New java.util.concurrent.atomic.AtomicLong(0)
		Private Shared ReadOnly mySpace As New UID
		Private Shared ReadOnly secureRandom As New java.security.SecureRandom

		''' <summary>
		''' @serial object number </summary>
		''' <seealso cref= #hashCode </seealso>
		Private ReadOnly objNum As Long

		''' <summary>
		''' @serial address space identifier (unique to host over time)
		''' </summary>
		Private ReadOnly space As UID

		''' <summary>
		''' Generates a unique object identifier.
		''' 
		''' <p>If the system property <code>java.rmi.server.randomIDs</code>
		''' is defined to equal the string <code>"true"</code> (case insensitive),
		''' then this constructor will use a cryptographically
		''' strong random number generator to choose the object number of the
		''' returned <code>ObjID</code>.
		''' </summary>
		Public Sub New()
	'        
	'         * If generating random object numbers, create a new UID to
	'         * ensure uniqueness; otherwise, use a shared UID because
	'         * sequential object numbers already ensure uniqueness.
	'         
			If useRandomIDs() Then
				space = New UID
				objNum = secureRandom.nextLong()
			Else
				space = mySpace
				objNum = nextObjNum.andIncrement
			End If
		End Sub

		''' <summary>
		''' Creates a "well-known" object identifier.
		''' 
		''' <p>An <code>ObjID</code> created via this constructor will not
		''' clash with any <code>ObjID</code>s generated via the no-arg
		''' constructor.
		''' </summary>
		''' <param name="objNum"> object number for well-known object identifier </param>
		Public Sub New(  objNum As Integer)
			space = New UID(CShort(0))
			Me.objNum = objNum
		End Sub

		''' <summary>
		''' Constructs an object identifier given data read from a stream.
		''' </summary>
		Private Sub New(  objNum As Long,   space As UID)
			Me.objNum = objNum
			Me.space = space
		End Sub

		''' <summary>
		''' Marshals a binary representation of this <code>ObjID</code> to
		''' an <code>ObjectOutput</code> instance.
		''' 
		''' <p>Specifically, this method first invokes the given stream's
		''' <seealso cref="ObjectOutput#writeLong(long)"/> method with this object
		''' identifier's object number, and then it writes its address
		''' space identifier by invoking its <seealso cref="UID#write(DataOutput)"/>
		''' method with the stream.
		''' </summary>
		''' <param name="out"> the <code>ObjectOutput</code> instance to write
		''' this <code>ObjID</code> to
		''' </param>
		''' <exception cref="IOException"> if an I/O error occurs while performing
		''' this operation </exception>
		Public Sub write(  out As java.io.ObjectOutput)
			out.writeLong(objNum)
			space.write(out)
		End Sub

		''' <summary>
		''' Constructs and returns a new <code>ObjID</code> instance by
		''' unmarshalling a binary representation from an
		''' <code>ObjectInput</code> instance.
		''' 
		''' <p>Specifically, this method first invokes the given stream's
		''' <seealso cref="ObjectInput#readLong()"/> method to read an object number,
		''' then it invokes <seealso cref="UID#read(DataInput)"/> with the
		''' stream to read an address space identifier, and then it
		''' creates and returns a new <code>ObjID</code> instance that
		''' contains the object number and address space identifier that
		''' were read from the stream.
		''' </summary>
		''' <param name="in"> the <code>ObjectInput</code> instance to read
		''' <code>ObjID</code> from
		''' </param>
		''' <returns>  unmarshalled <code>ObjID</code> instance
		''' </returns>
		''' <exception cref="IOException"> if an I/O error occurs while performing
		''' this operation </exception>
		Public Shared Function read(  [in] As java.io.ObjectInput) As ObjID
			Dim num As Long = [in].readLong()
			Dim space As UID = UID.read([in])
			Return New ObjID(num, space)
		End Function

		''' <summary>
		''' Returns the hash code value for this object identifier, the
		''' object number.
		''' </summary>
		''' <returns>  the hash code value for this object identifier </returns>
		Public Overrides Function GetHashCode() As Integer
			Return CInt(objNum)
		End Function

		''' <summary>
		''' Compares the specified object with this <code>ObjID</code> for
		''' equality.
		''' 
		''' This method returns <code>true</code> if and only if the
		''' specified object is an <code>ObjID</code> instance with the same
		''' object number and address space identifier as this one.
		''' </summary>
		''' <param name="obj"> the object to compare this <code>ObjID</code> to
		''' </param>
		''' <returns>  <code>true</code> if the given object is equivalent to
		''' this one, and <code>false</code> otherwise </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If TypeOf obj Is ObjID Then
				Dim id As ObjID = CType(obj, ObjID)
				Return objNum = id.objNum AndAlso space.Equals(id.space)
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns a string representation of this object identifier.
		''' </summary>
		''' <returns>  a string representation of this object identifier </returns>
	'    
	'     * The address space identifier is only included in the string
	'     * representation if it does not denote the local address space
	'     * (or if the randomIDs property was set).
	'     
		Public Overrides Function ToString() As String
			Return "[" & (If(space.Equals(mySpace), "", space & ", ")) + objNum & "]"
		End Function

		Private Shared Function useRandomIDs() As Boolean
			Dim value As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.rmi.server.randomIDs"))
			Return If(value Is Nothing, True, Convert.ToBoolean(value))
		End Function
	End Class

End Namespace