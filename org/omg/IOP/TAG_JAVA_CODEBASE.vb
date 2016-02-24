Namespace org.omg.IOP


	''' <summary>
	''' org/omg/IOP/TAG_JAVA_CODEBASE.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/IOP.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public Interface TAG_JAVA_CODEBASE

	  ''' <summary>
	  ''' Class downloading is supported for stubs, ties, values, and 
	  ''' value helpers. The specification allows transmission of codebase 
	  ''' information on the wire for stubs and ties, and enables usage of 
	  ''' pre-existing ClassLoaders when relevant.  
	  ''' <p>
	  ''' For values and value helpers, the codebase is transmitted after the 
	  ''' value tag.  For stubs and ties, the codebase is transmitted as 
	  ''' the TaggedComponent <code>TAG_JAVA_CODEBASE</code> in the IOR 
	  ''' profile, where the <code>component_data</code> is a CDR encapsulation 
	  ''' of the codebase written as an IDL string. The codebase is a 
	  ''' space-separated list of one or more URLs.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int value = (int)(25L);
	End Interface

End Namespace