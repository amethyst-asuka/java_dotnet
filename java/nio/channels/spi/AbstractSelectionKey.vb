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

Namespace java.nio.channels.spi



	''' <summary>
	''' Base implementation class for selection keys.
	''' 
	''' <p> This class tracks the validity of the key and implements cancellation.
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class AbstractSelectionKey
		Inherits SelectionKey

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private valid As Boolean = True

		Public Property NotOverridable Overrides valid As Boolean
			Get
				Return valid
			End Get
		End Property

		Friend Overridable Sub invalidate() ' package-private
			valid = False
		End Sub

		''' <summary>
		''' Cancels this key.
		''' 
		''' <p> If this key has not yet been cancelled then it is added to its
		''' selector's cancelled-key set while synchronized on that set.  </p>
		''' </summary>
		Public NotOverridable Overrides Sub cancel()
			' Synchronizing "this" to prevent this key from getting canceled
			' multiple times by different threads, which might cause race
			' condition between selector's select() and channel's close().
			SyncLock Me
				If valid Then
					valid = False
					CType(selector(), AbstractSelector).cancel(Me)
				End If
			End SyncLock
		End Sub
	End Class

End Namespace