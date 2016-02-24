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
' * File: ./org/omg/CosNaming/_NamingContextImplBase.java
' * From: nameservice.idl
' * Date: Tue Aug 11 03:12:09 1998
' *   By: idltojava Java IDL 1.2 Aug 11 1998 02:00:18
' * @deprecated Deprecated in JDK 1.4.
' 

Namespace org.omg.CosNaming
	Public MustInherit Class _NamingContextImplBase
		Inherits org.omg.CORBA.DynamicImplementation
		Implements org.omg.CosNaming.NamingContext

			Public MustOverride Sub destroy()
			Public MustOverride Function bind_new_context(ByVal n As org.omg.CosNaming.NameComponent()) As org.omg.CosNaming.NamingContext
			Public MustOverride Function new_context() As org.omg.CosNaming.NamingContext
			Public MustOverride Sub list(ByVal how_many As Integer, ByVal bl As org.omg.CosNaming.BindingListHolder, ByVal bi As org.omg.CosNaming.BindingIteratorHolder)
			Public MustOverride Sub unbind(ByVal n As org.omg.CosNaming.NameComponent())
			Public MustOverride Function resolve(ByVal n As org.omg.CosNaming.NameComponent()) As org.omg.CORBA.Object
			Public MustOverride Sub rebind_context(ByVal n As org.omg.CosNaming.NameComponent(), ByVal nc As org.omg.CosNaming.NamingContext)
			Public MustOverride Sub rebind(ByVal n As org.omg.CosNaming.NameComponent(), ByVal obj As org.omg.CORBA.Object)
			Public MustOverride Sub bind_context(ByVal n As org.omg.CosNaming.NameComponent(), ByVal nc As org.omg.CosNaming.NamingContext)
			Public MustOverride Sub bind(ByVal n As org.omg.CosNaming.NameComponent(), ByVal obj As org.omg.CORBA.Object)
		' Constructor
		Public Sub New()
			MyBase.New()
		End Sub
		' Type strings for this class and its superclases
		Private Shared ReadOnly _type_ids As String() = { "IDL:omg.org/CosNaming/NamingContext:1.0" }

		Public Overrides Function _ids() As String()
			Return CType(_type_ids.clone(), String())
		End Function

		Private Shared _methods As java.util.Dictionary = New Hashtable
		Shared Sub New()
			_methods.put("bind", New Integer?(0))
			_methods.put("bind_context", New Integer?(1))
			_methods.put("rebind", New Integer?(2))
			_methods.put("rebind_context", New Integer?(3))
			_methods.put("resolve", New Integer?(4))
			_methods.put("unbind", New Integer?(5))
			_methods.put("list", New Integer?(6))
			_methods.put("new_context", New Integer?(7))
			_methods.put("bind_new_context", New Integer?(8))
			_methods.put("destroy", New Integer?(9))
		End Sub
		' DSI Dispatch call
		Public Overridable Sub invoke(ByVal r As org.omg.CORBA.ServerRequest)
			Select Case CInt(Fix(_methods.get(r.op_name())))
			Case 0 ' org.omg.CosNaming.NamingContext.bind
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					Dim _obj As org.omg.CORBA.Any = _orb().create_any()
					_obj.type(org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_objref))
					_list.add_value("obj", _obj, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Dim obj As org.omg.CORBA.Object
					obj = _obj.extract_Object()
					Try
						Me.bind(n, obj)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e2)
						r.except(_except)
						Return
					Catch e3 As org.omg.CosNaming.NamingContextPackage.AlreadyBound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.AlreadyBoundHelper.insert(_except, e3)
						r.except(_except)
						Return
					End Try
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case 1 ' org.omg.CosNaming.NamingContext.bind_context
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					Dim _nc As org.omg.CORBA.Any = _orb().create_any()
					_nc.type(org.omg.CosNaming.NamingContextHelper.type())
					_list.add_value("nc", _nc, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Dim nc As org.omg.CosNaming.NamingContext
					nc = org.omg.CosNaming.NamingContextHelper.extract(_nc)
					Try
						Me.bind_context(n, nc)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e2)
						r.except(_except)
						Return
					Catch e3 As org.omg.CosNaming.NamingContextPackage.AlreadyBound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.AlreadyBoundHelper.insert(_except, e3)
						r.except(_except)
						Return
					End Try
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case 2 ' org.omg.CosNaming.NamingContext.rebind
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					Dim _obj As org.omg.CORBA.Any = _orb().create_any()
					_obj.type(org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_objref))
					_list.add_value("obj", _obj, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Dim obj As org.omg.CORBA.Object
					obj = _obj.extract_Object()
					Try
						Me.rebind(n, obj)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e2)
						r.except(_except)
						Return
					End Try
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case 3 ' org.omg.CosNaming.NamingContext.rebind_context
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					Dim _nc As org.omg.CORBA.Any = _orb().create_any()
					_nc.type(org.omg.CosNaming.NamingContextHelper.type())
					_list.add_value("nc", _nc, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Dim nc As org.omg.CosNaming.NamingContext
					nc = org.omg.CosNaming.NamingContextHelper.extract(_nc)
					Try
						Me.rebind_context(n, nc)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e2)
						r.except(_except)
						Return
					End Try
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case 4 ' org.omg.CosNaming.NamingContext.resolve
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Dim ___result As org.omg.CORBA.Object
					Try
						___result = Me.resolve(n)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e2)
						r.except(_except)
						Return
					End Try
					Dim __result As org.omg.CORBA.Any = _orb().create_any()
					__result.insert_Object(___result)
					r.result(__result)
			Case 5 ' org.omg.CosNaming.NamingContext.unbind
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Try
						Me.unbind(n)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e2)
						r.except(_except)
						Return
					End Try
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case 6 ' org.omg.CosNaming.NamingContext.list
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _how_many As org.omg.CORBA.Any = _orb().create_any()
					_how_many.type(org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_ulong))
					_list.add_value("how_many", _how_many, org.omg.CORBA.ARG_IN.value)
					Dim _bl As org.omg.CORBA.Any = _orb().create_any()
					_bl.type(org.omg.CosNaming.BindingListHelper.type())
					_list.add_value("bl", _bl, org.omg.CORBA.ARG_OUT.value)
					Dim _bi As org.omg.CORBA.Any = _orb().create_any()
					_bi.type(org.omg.CosNaming.BindingIteratorHelper.type())
					_list.add_value("bi", _bi, org.omg.CORBA.ARG_OUT.value)
					r.params(_list)
					Dim how_many As Integer
					how_many = _how_many.extract_ulong()
					Dim bl As org.omg.CosNaming.BindingListHolder
					bl = New org.omg.CosNaming.BindingListHolder
					Dim bi As org.omg.CosNaming.BindingIteratorHolder
					bi = New org.omg.CosNaming.BindingIteratorHolder
					Me.list(how_many, bl, bi)
					org.omg.CosNaming.BindingListHelper.insert(_bl, bl.value)
					org.omg.CosNaming.BindingIteratorHelper.insert(_bi, bi.value)
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case 7 ' org.omg.CosNaming.NamingContext.new_context
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					r.params(_list)
					Dim ___result As org.omg.CosNaming.NamingContext
					___result = Me.new_context()
					Dim __result As org.omg.CORBA.Any = _orb().create_any()
					org.omg.CosNaming.NamingContextHelper.insert(__result, ___result)
					r.result(__result)
			Case 8 ' org.omg.CosNaming.NamingContext.bind_new_context
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					Dim _n As org.omg.CORBA.Any = _orb().create_any()
					_n.type(org.omg.CosNaming.NameHelper.type())
					_list.add_value("n", _n, org.omg.CORBA.ARG_IN.value)
					r.params(_list)
					Dim n As org.omg.CosNaming.NameComponent()
					n = org.omg.CosNaming.NameHelper.extract(_n)
					Dim ___result As org.omg.CosNaming.NamingContext
					Try
						___result = Me.bind_new_context(n)
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotFound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotFoundHelper.insert(_except, e0)
						r.except(_except)
						Return
					Catch e1 As org.omg.CosNaming.NamingContextPackage.AlreadyBound
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.AlreadyBoundHelper.insert(_except, e1)
						r.except(_except)
						Return
					Catch e2 As org.omg.CosNaming.NamingContextPackage.CannotProceed
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.CannotProceedHelper.insert(_except, e2)
						r.except(_except)
						Return
					Catch e3 As org.omg.CosNaming.NamingContextPackage.InvalidName
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.InvalidNameHelper.insert(_except, e3)
						r.except(_except)
						Return
					End Try
					Dim __result As org.omg.CORBA.Any = _orb().create_any()
					org.omg.CosNaming.NamingContextHelper.insert(__result, ___result)
					r.result(__result)
			Case 9 ' org.omg.CosNaming.NamingContext.destroy
					Dim _list As org.omg.CORBA.NVList = _orb().create_list(0)
					r.params(_list)
					Try
						Me.destroy()
					Catch e0 As org.omg.CosNaming.NamingContextPackage.NotEmpty
						Dim _except As org.omg.CORBA.Any = _orb().create_any()
						org.omg.CosNaming.NamingContextPackage.NotEmptyHelper.insert(_except, e0)
						r.except(_except)
						Return
					End Try
					Dim __return As org.omg.CORBA.Any = _orb().create_any()
					__return.type(_orb().get_primitive_tc(org.omg.CORBA.TCKind.tk_void))
					r.result(__return)
			Case Else
				Throw New org.omg.CORBA.BAD_OPERATION(0, org.omg.CORBA.CompletionStatus.COMPLETED_MAYBE)
			End Select
		End Sub
	End Class

End Namespace