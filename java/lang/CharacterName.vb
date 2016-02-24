Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang


	Friend Class CharacterName

		Private Shared refStrPool As SoftReference(Of SByte())
		Private Shared lookup As Integer()()

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Function initNamePool() As SByte()
			Dim strPool As SByte() = Nothing
			strPool = refStrPool.get()
			If refStrPool IsNot Nothing AndAlso strPool IsNot Nothing Then Return strPool
			Dim dis As java.io.DataInputStream = Nothing
			Try
				dis = New java.io.DataInputStream(New java.util.zip.InflaterInputStream(java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

				lookup = New Integer((Character.MAX_CODE_POINT + 1) >> 8 - 1)(){}
				Dim total As Integer = dis.readInt()
				Dim cpEnd As Integer = dis.readInt()
				Dim ba As SByte() = New SByte(cpEnd - 1){}
				dis.readFully(ba)

				Dim nameOff As Integer = 0
				Dim cpOff As Integer = 0
				Dim cp As Integer = 0
				Do
					Dim len As Integer = ba(cpOff) And &Hff
					cpOff += 1
					If len = 0 Then
						len = ba(cpOff) And &Hff
						cpOff += 1
						' always big-endian
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						cp = ((ba(cpOff++) And &Hff) << 16) Or ((ba(cpOff++) And &Hff) << 8) Or ((ba(cpOff++) And &Hff))
					Else
						cp += 1
					End If
					Dim hi As Integer = cp >> 8
					If lookup(hi) Is Nothing Then lookup(hi) = New Integer(){}
					lookup(hi)(cp And &Hff) = (nameOff << 8) Or len
					nameOff += len
				Loop While cpOff < cpEnd
				strPool = New SByte(total - cpEnd - 1){}
				dis.readFully(strPool)
				refStrPool = New SoftReference(Of )(strPool)
			Catch x As Exception
				Throw New InternalError(x.message, x)
			Finally
				Try
					If dis IsNot Nothing Then dis.close()
				Catch xx As Exception
				End Try
			End Try
			Return strPool
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As java.io.InputStream
				Return Me.GetType().getResourceAsStream("uniName.dat")
			End Function
		End Class

		Public Shared Function [get](ByVal cp As Integer) As String
			Dim strPool As SByte() = Nothing
			strPool = refStrPool.get()
			If refStrPool Is Nothing OrElse strPool Is Nothing Then strPool = initNamePool()
			Dim [off] As Integer = 0
			[off] = lookup(cp>>8)(cp And &Hff)
			If lookup(cp>>8) Is Nothing OrElse [off] = 0 Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim result As New String(strPool, 0, CInt(CUInt([off]) >> 8), [off] And &Hff) ' ASCII
			Return result
		End Function
	End Class

End Namespace