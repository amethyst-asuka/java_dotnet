Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynAnyOperations.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Any values can be dynamically interpreted (traversed) and constructed through DynAny objects.
	''' A DynAny object is associated with a data value which corresponds to a copy of the value
	''' inserted into an any.
	''' <P>A DynAny object may be viewed as an ordered collection of component DynAnys.
	''' For DynAnys representing a basic type, such as long, or a type without components,
	''' such as an empty exception, the ordered collection of components is empty.
	''' Each DynAny object maintains the notion of a current position into its collection
	''' of component DynAnys. The current position is identified by an index value that runs
	''' from 0 to n-1, where n is the number of components.
	''' The special index value -1 indicates a current position that points nowhere.
	''' For values that cannot have a current position (such as an empty exception),
	''' the index value is fixed at -1.
	''' If a DynAny is initialized with a value that has components, the index is initialized to 0.
	''' After creation of an uninitialized DynAny (that is, a DynAny that has no value but a TypeCode
	''' that permits components), the current position depends on the type of value represented by
	''' the DynAny. (The current position is set to 0 or -1, depending on whether the new DynAny
	''' gets default values for its components.)
	''' <P>The iteration operations rewind, seek, and next can be used to change the current position
	''' and the current_component operation returns the component at the current position.
	''' The component_count operation returns the number of components of a DynAny.
	''' Collectively, these operations enable iteration over the components of a DynAny, for example,
	''' to (recursively) examine its contents.
	''' <P>A constructed DynAny object is a DynAny object associated with a constructed type.
	''' There is a different interface, inheriting from the DynAny interface, associated with
	''' each kind of constructed type in IDL (fixed, enum, struct, sequence, union, array,
	''' exception, and value type).
	''' <P>A constructed DynAny object exports operations that enable the creation of new DynAny objects,
	''' each of them associated with a component of the constructed data value.
	''' As an example, a DynStruct is associated with a struct value. This means that the DynStruct
	''' may be seen as owning an ordered collection of components, one for each structure member.
	''' The DynStruct object exports operations that enable the creation of new DynAny objects,
	''' each of them associated with a member of the struct.
	''' <P>If a DynAny object has been obtained from another (constructed) DynAny object,
	''' such as a DynAny representing a structure member that was created from a DynStruct,
	''' the member DynAny is logically contained in the DynStruct.
	''' Calling an insert or get operation leaves the current position unchanged.
	''' Destroying a top-level DynAny object (one that was not obtained as a component of another DynAny)
	''' also destroys any component DynAny objects obtained from it.
	''' Destroying a non-top level DynAny object does nothing.
	''' Invoking operations on a destroyed top-level DynAny or any of its descendants raises OBJECT_NOT_EXIST.
	''' If the programmer wants to destroy a DynAny object but still wants to manipulate some component
	''' of the data value associated with it, then he or she should first create a DynAny for the component
	''' and, after that, make a copy of the created DynAny object.
	''' <P>The behavior of DynAny objects has been defined in order to enable efficient implementations
	''' in terms of allocated memory space and speed of access. DynAny objects are intended to be used
	''' for traversing values extracted from anys or constructing values of anys at runtime.
	''' Their use for other purposes is not recommended.
	''' <P>Insert and get operations are necessary to handle basic DynAny objects
	''' but are also helpful to handle constructed DynAny objects.
	''' Inserting a basic data type value into a constructed DynAny object
	''' implies initializing the current component of the constructed data value
	''' associated with the DynAny object. For example, invoking insert_boolean on a
	''' DynStruct implies inserting a boolean data value at the current position
	''' of the associated struct data value.
	''' A type is consistent for inserting or extracting a value if its TypeCode is equivalent to
	''' the TypeCode contained in the DynAny or, if the DynAny has components, is equivalent to the TypeCode
	''' of the DynAny at the current position.
	''' <P>DynAny and DynAnyFactory objects are intended to be local to the process in which they are
	''' created and used. This means that references to DynAny and DynAnyFactory objects cannot be exported
	''' to other processes, or externalized with ORB.object_to_string().
	''' If any attempt is made to do so, the offending operation will raise a MARSHAL system exception.
	''' Since their interfaces are specified in IDL, DynAny objects export operations defined in the standard
	''' org.omg.CORBA.Object interface. However, any attempt to invoke operations exported through the Object
	''' interface may raise the standard NO_IMPLEMENT exception.
	''' An attempt to use a DynAny object with the DII may raise the NO_IMPLEMENT exception.
	''' </summary>
	Public Interface DynAnyOperations

	  ''' <summary>
	  ''' Returns the TypeCode associated with this DynAny object.
	  ''' A DynAny object is created with a TypeCode value assigned to it.
	  ''' This TypeCode value determines the type of the value handled through the DynAny object.
	  ''' Note that the TypeCode associated with a DynAny object is initialized at the time the
	  ''' DynAny is created and cannot be changed during lifetime of the DynAny object.
	  ''' </summary>
	  ''' <returns> The TypeCode associated with this DynAny object </returns>
	  Function type() As org.omg.CORBA.TypeCode

	  ''' <summary>
	  ''' Initializes the value associated with a DynAny object with the value
	  ''' associated with another DynAny object.
	  ''' The current position of the target DynAny is set to zero for values that have components
	  ''' and to -1 for values that do not have components.
	  ''' </summary>
	  ''' <param name="dyn_any"> </param>
	  ''' <exception cref="TypeMismatch"> if the type of the passed DynAny is not equivalent to the type of target DynAny </exception>
	  Sub assign(ByVal dyn_any As org.omg.DynamicAny.DynAny)

	  ''' <summary>
	  ''' Initializes the value associated with a DynAny object with the value contained in an any.
	  ''' The current position of the target DynAny is set to zero for values that have components
	  ''' and to -1 for values that do not have components.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the type of the passed Any is not equivalent to the type of target DynAny </exception>
	  ''' <exception cref="InvalidValue"> if the passed Any does not contain a legal value (such as a null string) </exception>
	  Sub from_any(ByVal value As org.omg.CORBA.Any)

	  ''' <summary>
	  ''' Creates an any value from a DynAny object.
	  ''' A copy of the TypeCode associated with the DynAny object is assigned to the resulting any.
	  ''' The value associated with the DynAny object is copied into the any.
	  ''' </summary>
	  ''' <returns> a new Any object with the same value and TypeCode </returns>
	  Function to_any() As org.omg.CORBA.Any

	  ''' <summary>
	  ''' Compares two DynAny values for equality.
	  ''' Two DynAny values are equal if their TypeCodes are equivalent and, recursively, all component DynAnys
	  ''' have equal values.
	  ''' The current position of the two DynAnys being compared has no effect on the result of equal.
	  ''' </summary>
	  ''' <returns> true of the DynAnys are equal, false otherwise </returns>
	  Function equal(ByVal dyn_any As org.omg.DynamicAny.DynAny) As Boolean

	  ''' <summary>
	  ''' Destroys a DynAny object.
	  ''' This operation frees any resources used to represent the data value associated with a DynAny object.
	  ''' It must be invoked on references obtained from one of the creation operations on the ORB interface
	  ''' or on a reference returned by DynAny.copy() to avoid resource leaks.
	  ''' Invoking destroy on component DynAny objects (for example, on objects returned by the
	  ''' current_component operation) does nothing.
	  ''' Destruction of a DynAny object implies destruction of all DynAny objects obtained from it.
	  ''' That is, references to components of a destroyed DynAny become invalid.
	  ''' Invocations on such references raise OBJECT_NOT_EXIST.
	  ''' It is possible to manipulate a component of a DynAny beyond the life time of the DynAny
	  ''' from which the component was obtained by making a copy of the component with the copy operation
	  ''' before destroying the DynAny from which the component was obtained.
	  ''' </summary>
	  Sub destroy()

	  ''' <summary>
	  ''' Creates a new DynAny object whose value is a deep copy of the DynAny on which it is invoked.
	  ''' The operation is polymorphic, that is, invoking it on one of the types derived from DynAny,
	  ''' such as DynStruct, creates the derived type but returns its reference as the DynAny base type.
	  ''' </summary>
	  ''' <returns> a deep copy of the DynAny object </returns>
	  Function copy() As org.omg.DynamicAny.DynAny

	  ''' <summary>
	  ''' Inserts a boolean value into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_boolean(ByVal value As Boolean)

	  ''' <summary>
	  ''' Inserts a byte value into the DynAny. The IDL octet data type is mapped to the Java byte data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_octet(ByVal value As SByte)

	  ''' <summary>
	  ''' Inserts a char value into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_char(ByVal value As Char)

	  ''' <summary>
	  ''' Inserts a short value into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_short(ByVal value As Short)

	  ''' <summary>
	  ''' Inserts a short value into the DynAny. The IDL ushort data type is mapped to the Java short data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_ushort(ByVal value As Short)

	  ''' <summary>
	  ''' Inserts an integer value into the DynAny. The IDL long data type is mapped to the Java int data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_long(ByVal value As Integer)

	  ''' <summary>
	  ''' Inserts an integer value into the DynAny. The IDL ulong data type is mapped to the Java int data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_ulong(ByVal value As Integer)

	  ''' <summary>
	  ''' Inserts a float value into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_float(ByVal value As Single)

	  ''' <summary>
	  ''' Inserts a double value into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_double(ByVal value As Double)

	  ''' <summary>
	  ''' Inserts a string value into the DynAny.
	  ''' Both bounded and unbounded strings are inserted using this method.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="InvalidValue"> if the string inserted is longer than the bound of a bounded string </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_string(ByVal value As String)

	  ''' <summary>
	  ''' Inserts a reference to a CORBA object into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_reference(ByVal value As org.omg.CORBA.Object)

	  ''' <summary>
	  ''' Inserts a TypeCode object into the DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_typecode(ByVal value As org.omg.CORBA.TypeCode)

	  ''' <summary>
	  ''' Inserts a long value into the DynAny. The IDL long long data type is mapped to the Java long data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_longlong(ByVal value As Long)

	  ''' <summary>
	  ''' Inserts a long value into the DynAny.
	  ''' The IDL unsigned long long data type is mapped to the Java long data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_ulonglong(ByVal value As Long)

	  ''' <summary>
	  ''' Inserts a char value into the DynAny. The IDL wchar data type is mapped to the Java char data type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_wchar(ByVal value As Char)

	  ''' <summary>
	  ''' Inserts a string value into the DynAny.
	  ''' Both bounded and unbounded strings are inserted using this method.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="InvalidValue"> if the string inserted is longer than the bound of a bounded string </exception>
	  Sub insert_wstring(ByVal value As String)

	  ''' <summary>
	  ''' Inserts an Any value into the Any represented by this DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_any(ByVal value As org.omg.CORBA.Any)

	  ''' <summary>
	  ''' Inserts the Any value contained in the parameter DynAny into the Any represented by this DynAny.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_dyn_any(ByVal value As org.omg.DynamicAny.DynAny)

	  ''' <summary>
	  ''' Inserts a reference to a Serializable object into this DynAny.
	  ''' The IDL ValueBase type is mapped to the Java Serializable type.
	  ''' </summary>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Sub insert_val(ByVal value As java.io.Serializable)

	  ''' <summary>
	  ''' Extracts the boolean value from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_boolean() As Boolean

	  ''' <summary>
	  ''' Extracts the byte value from this DynAny. The IDL octet data type is mapped to the Java byte data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_octet() As SByte

	  ''' <summary>
	  ''' Extracts the char value from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_char() As Char

	  ''' <summary>
	  ''' Extracts the short value from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_short() As Short

	  ''' <summary>
	  ''' Extracts the short value from this DynAny. The IDL ushort data type is mapped to the Java short data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_ushort() As Short

	  ''' <summary>
	  ''' Extracts the integer value from this DynAny. The IDL long data type is mapped to the Java int data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_long() As Integer

	  ''' <summary>
	  ''' Extracts the integer value from this DynAny. The IDL ulong data type is mapped to the Java int data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_ulong() As Integer

	  ''' <summary>
	  ''' Extracts the float value from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_float() As Single

	  ''' <summary>
	  ''' Extracts the double value from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_double() As Double

	  ''' <summary>
	  ''' Extracts the string value from this DynAny.
	  ''' Both bounded and unbounded strings are extracted using this method.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_string() As String

	  ''' <summary>
	  ''' Extracts the reference to a CORBA Object from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_reference() As org.omg.CORBA.Object

	  ''' <summary>
	  ''' Extracts the TypeCode object from this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_typecode() As org.omg.CORBA.TypeCode

	  ''' <summary>
	  ''' Extracts the long value from this DynAny. The IDL long long data type is mapped to the Java long data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_longlong() As Long

	  ''' <summary>
	  ''' Extracts the long value from this DynAny.
	  ''' The IDL unsigned long long data type is mapped to the Java long data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_ulonglong() As Long

	  ''' <summary>
	  ''' Extracts the long value from this DynAny. The IDL wchar data type is mapped to the Java char data type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_wchar() As Char

	  ''' <summary>
	  ''' Extracts the string value from this DynAny.
	  ''' Both bounded and unbounded strings are extracted using this method.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  Function get_wstring() As String

	  ''' <summary>
	  ''' Extracts an Any value contained in the Any represented by this DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_any() As org.omg.CORBA.Any

	  ''' <summary>
	  ''' Extracts the Any value contained in the Any represented by this DynAny and returns it wrapped
	  ''' into a new DynAny.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_dyn_any() As org.omg.DynamicAny.DynAny

	  ''' <summary>
	  ''' Extracts a Serializable object from this DynAny.
	  ''' The IDL ValueBase type is mapped to the Java Serializable type.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> if the accessed component in the DynAny is of a type
	  ''' that is not equivalent to the requested type. </exception>
	  ''' <exception cref="TypeMismatch"> if called on a DynAny whose current component itself has components </exception>
	  ''' <exception cref="InvalidValue"> if this DynAny has components but has a current position of -1 </exception>
	  Function get_val() As java.io.Serializable

	  ''' <summary>
	  ''' Sets the current position to index. The current position is indexed 0 to n-1, that is,
	  ''' index zero corresponds to the first component. The operation returns true if the resulting
	  ''' current position indicates a component of the DynAny and false if index indicates
	  ''' a position that does not correspond to a component.
	  ''' Calling seek with a negative index is legal. It sets the current position to -1 to indicate
	  ''' no component and returns false. Passing a non-negative index value for a DynAny that does not
	  ''' have a component at the corresponding position sets the current position to -1 and returns false.
	  ''' </summary>
	  Function seek(ByVal index As Integer) As Boolean

	  ''' <summary>
	  ''' Is equivalent to seek(0).
	  ''' </summary>
	  Sub rewind()

	  ''' <summary>
	  ''' Advances the current position to the next component.
	  ''' The operation returns true while the resulting current position indicates a component, false otherwise.
	  ''' A false return value leaves the current position at -1.
	  ''' Invoking next on a DynAny without components leaves the current position at -1 and returns false.
	  ''' </summary>
	  Function [next]() As Boolean

	  ''' <summary>
	  ''' Returns the number of components of a DynAny.
	  ''' For a DynAny without components, it returns zero.
	  ''' The operation only counts the components at the top level.
	  ''' For example, if component_count is invoked on a DynStruct with a single member,
	  ''' the return value is 1, irrespective of the type of the member.
	  ''' <UL>
	  ''' <LI>For sequences, the operation returns the current number of elements.
	  ''' <LI>For structures, exceptions, and value types, the operation returns the number of members.
	  ''' <LI>For arrays, the operation returns the number of elements.
	  ''' <LI>For unions, the operation returns 2 if the discriminator indicates that a named member is active,
	  ''' otherwise, it returns 1.
	  ''' <LI>For DynFixed and DynEnum, the operation returns zero.
	  ''' </UL>
	  ''' </summary>
	  Function component_count() As Integer

	  ''' <summary>
	  ''' Returns the DynAny for the component at the current position.
	  ''' It does not advance the current position, so repeated calls to current_component
	  ''' without an intervening call to rewind, next, or seek return the same component.
	  ''' The returned DynAny object reference can be used to get/set the value of the current component.
	  ''' If the current component represents a complex type, the returned reference can be narrowed
	  ''' based on the TypeCode to get the interface corresponding to the to the complex type.
	  ''' Calling current_component on a DynAny that cannot have components,
	  ''' such as a DynEnum or an empty exception, raises TypeMismatch.
	  ''' Calling current_component on a DynAny whose current position is -1 returns a nil reference.
	  ''' The iteration operations, together with current_component, can be used
	  ''' to dynamically compose an any value. After creating a dynamic any, such as a DynStruct,
	  ''' current_component and next can be used to initialize all the components of the value.
	  ''' Once the dynamic value is completely initialized, to_any creates the corresponding any value.
	  ''' </summary>
	  ''' <exception cref="TypeMismatch"> If called on a DynAny that cannot have components,
	  ''' such as a DynEnum or an empty exception </exception>
	  Function current_component() As org.omg.DynamicAny.DynAny
	End Interface ' interface DynAnyOperations

End Namespace