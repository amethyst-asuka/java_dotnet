'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA

	''' <summary>
	'''  Provides the <tt>DomainManager</tt> with the means to access policies.
	'''  <P>
	'''  The <tt>DomainManager</tt> has associated with it the policy objects for a
	'''  particular domain. The domain manager also records the membership of
	'''  the domain and provides the means to add and remove members. The domain
	'''  manager is itself a member of a domain, possibly the domain it manages.
	'''  The domain manager provides mechanisms for establishing and navigating
	'''  relationships to superior and subordinate domains and
	'''  creating and accessing policies.
	''' </summary>

	Public Interface DomainManagerOperations
		''' <summary>
		''' This returns the policy of the specified type for objects in
		'''  this domain.  The types of policies available are domain specific.
		'''  See the CORBA specification for a list of standard ORB policies.
		''' </summary>
		''' <param name="policy_type"> Type of policy to request </param>
		Function get_domain_policy(ByVal policy_type As Integer) As org.omg.CORBA.Policy
	End Interface

End Namespace