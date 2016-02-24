Imports Microsoft.VisualBasic
Imports System.Collections

'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
' * File: ./org/omg/CosNaming/_BindingIteratorImplBase.java
' * From: nameservice.idl
' * Date: Tue Aug 11 03:12:09 1998
' *   By: idltojava Java IDL 1.2 Aug 11 1998 02:00:18
' * @deprecated Deprecated in JDK 1.4.
' 

Namespace org.omg.CosNaming
	Public MustInherit Class _BindingIteratorImplBase
		Inherits org.omg.CORBA.DynamicImplementation
		Implements org.omg.CosNaming.BindingIterator

			Public MustOverride Sub destroy()
			Public MustOverride Function next_n(ByVal how_many As Integer, ByVal bl As org.omg.CosNaming.BindingListHolder) As Boolean
			Public MustOverride Function next_one(ByVal b As org.omg.CosNaming.BindingHolder) As Boolean
		' Constructor
		Public Sub New()
			MyBase.New()
		End Sub
		' Type strings for this class and its superclases
		Private Shared ReadOnly _type_ids As String() = { "IDL:omg.org/CosNaming/BindingIterator:1.0" }

		Public Overrides Function _ids() As String()
			Return CType(_type_ids.clone(), String())
		End Function

		Private Shared _methods As java.util.Dictionary = New Hashtable
		Shared Sub New()
			_methods.put("next_one", New Integer?(0))
			_methods.put("next_n", New Integer?(1))
			_methods.put("destroy", New Integer?(2))
		End Sub
		' DSI Dispatch call
		Public Overridable Sub invoke(ByVal r As org.omg.CORBA.ServerRequest)
			Select Case CInt(Fix(_methods.get(r.op_name())))
			Case 0 ' org.omg.CosNaming.BindingIterator.next_one
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _b As org.omg.CORBA.Any = _orb().create_any()
					_b.type(org.omg.CosNaming.BindingHelper.type())
					_list.add_value("b", _b, org.omg.CORBA.ARG_OUT.value)
					r.params(_list)
					Dim b As org.omg.CosNaming.BindingHolder
					b = New org.omg.CosNaming.BindingHolder
					Dim ___result As Boolean
					___result = Me.next_one(b)
					org.omg.CosNaming.BindingHelper.insert(_b, b.value)
					Dim __result As org.omg.CORBA.Any = _orb().create_any()
					__result.insert_boolean(___result)
					r.result(__result)
			Case 1 ' org.omg.CosNaming.BindingIterator.next_n
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _how_many As org.omg.CORBA.Any = _orb().create_any()
					_how_many.type(org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_ulong))
					_list.add_value("how_many", _how_many, org.omg.CORBA.ARG_IN.value)
					Dim _bl As org.omg.CORBA.Any = _orb().create_any()
					_bl.type(org.omg.CosNaming.BindingListHelper.type())
					_list.add_value("bl", _bl, org.omg.CORBA.ARG_OUT.value)
					r.params(_list)
					Dim how_many As Integer
					how_many = _how_many.extract_ulong()
					Dim bl As org.omg.CosNaming.BindingListHolder
					bl = New org.omg.CosNaming.BindingListHolder
					Dim ___result As Boolean
					___result = Me.next_n(how_many, bl)
					org.omg.CosNaming.BindingListHelper.insert(_bl, bl.value)
					Dim __result As org.omg.CORBA.Any = _orb().create_any()
					__result.insert_boolean(___result)
					r.result(__result)
			Case 2 ' org.omg.CosNaming.BindingIterator.destroy
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					r.params(_list)
					Me.destroy()
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case Else
				Throw New org.omg.CORBA.BAD_OPERATION(0, org.omg.CORBA.CompletionStatus.COMPLETED_MAYBE)
			End Select
		End Sub
	End Class

End Namespace