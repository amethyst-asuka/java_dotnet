Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.spi

	''' <summary>
	''' An optional interface that may be provided by service provider
	''' objects that will be registered with a
	''' <code>ServiceRegistry</code>.  If this interface is present,
	''' notification of registration and deregistration will be performed.
	''' </summary>
	''' <seealso cref= ServiceRegistry
	'''  </seealso>
	Public Interface RegisterableService

		''' <summary>
		''' Called when an object implementing this interface is added to
		''' the given <code>category</code> of the given
		''' <code>registry</code>.  The object may already be registered
		''' under another category or categories.
		''' </summary>
		''' <param name="registry"> a <code>ServiceRegistry</code> where this
		''' object has been registered. </param>
		''' <param name="category"> a <code>Class</code> object indicating the
		''' registry category under which this object has been registered. </param>
		Sub onRegistration(ByVal registry As ServiceRegistry, ByVal category As Type)

		''' <summary>
		''' Called when an object implementing this interface is removed
		''' from the given <code>category</code> of the given
		''' <code>registry</code>.  The object may still be registered
		''' under another category or categories.
		''' </summary>
		''' <param name="registry"> a <code>ServiceRegistry</code> from which this
		''' object is being (wholly or partially) deregistered. </param>
		''' <param name="category"> a <code>Class</code> object indicating the
		''' registry category from which this object is being deregistered. </param>
		Sub onDeregistration(ByVal registry As ServiceRegistry, ByVal category As Type)
	End Interface

End Namespace