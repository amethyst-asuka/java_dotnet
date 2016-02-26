Imports System

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

Namespace javax.print




	''' <summary>
	''' Interface PrintService is the factory for a DocPrintJob. A PrintService
	''' describes the capabilities of a Printer and can be queried regarding
	''' a printer's supported attributes.
	''' <P>
	''' Example:
	'''   <PRE>{@code
	'''   DocFlavor flavor = DocFlavor.INPUT_STREAM.POSTSCRIPT;
	'''   PrintRequestAttributeSet aset = new HashPrintRequestAttributeSet();
	'''   aset.add(MediaSizeName.ISO_A4);
	'''   PrintService[] pservices =
	'''                 PrintServiceLookup.lookupPrintServices(flavor, aset);
	'''   if (pservices.length > 0) {
	'''       DocPrintJob pj = pservices[0].createPrintJob();
	'''       try {
	'''           FileInputStream fis = new FileInputStream("test.ps");
	'''           Doc doc = new SimpleDoc(fis, flavor, null);
	'''           pj.print(doc, aset);
	'''        } catch (FileNotFoundException fe) {
	'''        } catch (PrintException e) {
	'''        }
	'''   }
	'''   }</PRE>
	''' </summary>
	Public Interface PrintService

		''' <summary>
		''' Returns a String name for this print service which may be used
		''' by applications to request a particular print service.
		''' In a suitable context, such as a name service, this name must be
		''' unique.
		''' In some environments this unique name may be the same as the user
		''' friendly printer name defined as the
		''' <seealso cref="javax.print.attribute.standard.PrinterName PrinterName"/>
		''' attribute. </summary>
		''' <returns> name of the service. </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Creates and returns a PrintJob capable of handling data from
		''' any of the supported document flavors. </summary>
		''' <returns> a DocPrintJob object </returns>
		Function createPrintJob() As DocPrintJob

		''' <summary>
		''' Registers a listener for events on this PrintService. </summary>
		''' <param name="listener">  a PrintServiceAttributeListener, which
		'''        monitors the status of a print service </param>
		''' <seealso cref= #removePrintServiceAttributeListener </seealso>
		Sub addPrintServiceAttributeListener(ByVal listener As javax.print.event.PrintServiceAttributeListener)

		''' <summary>
		''' Removes the print-service listener from this print service.
		''' This means the listener is no longer interested in
		''' <code>PrintService</code> events. </summary>
		''' <param name="listener">  a PrintServiceAttributeListener object </param>
		''' <seealso cref= #addPrintServiceAttributeListener </seealso>
		Sub removePrintServiceAttributeListener(ByVal listener As javax.print.event.PrintServiceAttributeListener)

		''' <summary>
		''' Obtains this print service's set of printer description attributes
		''' giving this Print Service's status. The returned attribute set object
		''' is unmodifiable. The returned attribute set object is a "snapshot" of
		''' this Print Service's attribute set at the time of the
		''' <CODE>getAttributes()</CODE> method call: that is, the returned
		''' attribute set's contents will <I>not</I> be updated if this print
		''' service's attribute set's contents change in the future. To detect
		''' changes in attribute values, call <CODE>getAttributes()</CODE> again
		''' and compare the new attribute set to the previous attribute set;
		''' alternatively, register a listener for print service events.
		''' </summary>
		''' <returns>  Unmodifiable snapshot of this Print Service's attribute set.
		'''          May be empty, but not null. </returns>
		ReadOnly Property attributes As javax.print.attribute.PrintServiceAttributeSet

		''' <summary>
		''' Gets the value of the single specified service attribute.
		''' This may be useful to clients which only need the value of one
		''' attribute and want to minimize overhead. </summary>
		''' <param name="category"> the category of a PrintServiceAttribute supported
		''' by this service - may not be null. </param>
		''' <returns> the value of the supported attribute or null if the
		''' attribute is not supported by this service. </returns>
		''' <exception cref="NullPointerException"> if the category is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) if <CODE>category</CODE> is not a
		'''     <code>Class</code> that implements interface
		''' <seealso cref="javax.print.attribute.PrintServiceAttribute PrintServiceAttribute"/>. </exception>
		 Function getAttribute(Of T As javax.print.attribute.PrintServiceAttribute)(ByVal category As Type) As T

		''' <summary>
		''' Determines the print data formats a client can specify when setting
		''' up a job for this <code>PrintService</code>. A print data format is
		''' designated by a "doc
		''' flavor" (class {@link javax.print.DocFlavor DocFlavor})
		''' consisting of a MIME type plus a print data representation class.
		''' <P>
		''' Note that some doc flavors may not be supported in combination
		''' with all attributes. Use <code>getUnsupportedAttributes(..)</code>
		''' to validate specific combinations.
		''' </summary>
		''' <returns>  Array of supported doc flavors, should have at least
		'''          one element.
		'''  </returns>
		ReadOnly Property supportedDocFlavors As DocFlavor()

		''' <summary>
		''' Determines if this print service supports a specific
		''' <code>DocFlavor</code>.  This is a convenience method to determine
		''' if the <code>DocFlavor</code> would be a member of the result of
		''' <code>getSupportedDocFlavors()</code>.
		''' <p>
		''' Note that some doc flavors may not be supported in combination
		''' with all attributes. Use <code>getUnsupportedAttributes(..)</code>
		''' to validate specific combinations.
		''' </summary>
		''' <param name="flavor"> the <code>DocFlavor</code>to query for support. </param>
		''' <returns>  <code>true</code> if this print service supports the
		''' specified <code>DocFlavor</code>; <code>false</code> otherwise. </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>flavor</CODE> is null. </exception>
		Function isDocFlavorSupported(ByVal flavor As DocFlavor) As Boolean


		''' <summary>
		''' Determines the printing attribute categories a client can specify
		''' when setting up a job for this print service.
		''' A printing attribute category is
		''' designated by a <code>Class</code> that implements interface
		''' <seealso cref="javax.print.attribute.Attribute Attribute"/>. This method returns
		''' just the attribute <I>categories</I> that are supported; it does not
		''' return the particular attribute <I>values</I> that are supported.
		''' <P>
		''' This method returns all the printing attribute
		''' categories this print service supports for any possible job.
		''' Some categories may not be supported in a particular context (ie
		''' for a particular <code>DocFlavor</code>).
		''' Use one of the methods that include a <code>DocFlavor</code> to
		''' validate the request before submitting it, such as
		''' <code>getSupportedAttributeValues(..)</code>.
		''' </summary>
		''' <returns>  Array of printing attribute categories that the client can
		'''          specify as a doc-level or job-level attribute in a Print
		'''          Request. Each element in the array is a {@link java.lang.Class
		'''          Class} that implements interface {@link
		'''          javax.print.attribute.Attribute Attribute}.
		'''          The array is empty if no categories are supported. </returns>
		ReadOnly Property supportedAttributeCategories As Type()

		''' <summary>
		''' Determines whether a client can specify the given printing
		''' attribute category when setting up a job for this print service. A
		''' printing attribute category is designated by a <code>Class</code>
		''' that implements interface {@link javax.print.attribute.Attribute
		''' Attribute}. This method tells whether the attribute <I>category</I> is
		''' supported; it does not tell whether a particular attribute <I>value</I>
		''' is supported.
		''' <p>
		''' Some categories may not be supported in a particular context (ie
		''' for a particular <code>DocFlavor</code>).
		''' Use one of the methods which include a <code>DocFlavor</code> to
		''' validate the request before submitting it, such as
		''' <code>getSupportedAttributeValues(..)</code>.
		''' <P>
		''' This is a convenience method to determine if the category
		''' would be a member of the result of
		''' <code>getSupportedAttributeCategories()</code>.
		''' </summary>
		''' <param name="category">    Printing attribute category to test. It must be a
		'''                        <code>Class</code> that implements
		'''                        interface
		'''                <seealso cref="javax.print.attribute.Attribute Attribute"/>.
		''' </param>
		''' <returns>  <code>true</code> if this print service supports
		'''          specifying a doc-level or
		'''          job-level attribute in <CODE>category</CODE> in a Print
		'''          Request; <code>false</code> if it doesn't.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>category</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>category</CODE> is not a
		'''     <code>Class</code> that implements interface
		'''     <seealso cref="javax.print.attribute.Attribute Attribute"/>. </exception>
		Function isAttributeCategorySupported(ByVal category As Type) As Boolean

		''' <summary>
		''' Determines this print service's default printing attribute value in
		''' the given category. A printing attribute value is an instance of
		''' a class that implements interface
		''' <seealso cref="javax.print.attribute.Attribute Attribute"/>. If a client sets
		''' up a print job and does not specify any attribute value in the
		''' given category, this Print Service will use the
		''' default attribute value instead.
		''' <p>
		''' Some attributes may not be supported in a particular context (ie
		''' for a particular <code>DocFlavor</code>).
		''' Use one of the methods that include a <code>DocFlavor</code> to
		''' validate the request before submitting it, such as
		''' <code>getSupportedAttributeValues(..)</code>.
		''' <P>
		''' Not all attributes have a default value. For example the
		''' service will not have a defaultvalue for <code>RequestingUser</code>
		''' i.e. a null return for a supported category means there is no
		''' service default value for that category. Use the
		''' <code>isAttributeCategorySupported(Class)</code> method to
		''' distinguish these cases.
		''' </summary>
		''' <param name="category">    Printing attribute category for which the default
		'''                     attribute value is requested. It must be a {@link
		'''                        java.lang.Class Class} that implements interface
		'''                        {@link javax.print.attribute.Attribute
		'''                        Attribute}.
		''' </param>
		''' <returns>  Default attribute value for <CODE>category</CODE>, or null
		'''       if this Print Service does not support specifying a doc-level or
		'''          job-level attribute in <CODE>category</CODE> in a Print
		'''          Request, or the service does not have a default value
		'''          for this attribute.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>category</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>category</CODE> is not a
		'''     <seealso cref="java.lang.Class Class"/> that implements interface {@link
		'''     javax.print.attribute.Attribute Attribute}. </exception>
		Function getDefaultAttributeValue(ByVal category As Type) As Object

		''' <summary>
		''' Determines the printing attribute values a client can specify in
		''' the given category when setting up a job for this print service. A
		''' printing
		''' attribute value is an instance of a class that implements interface
		''' <seealso cref="javax.print.attribute.Attribute Attribute"/>.
		''' <P>
		''' If <CODE>flavor</CODE> is null and <CODE>attributes</CODE> is null
		''' or is an empty set, this method returns all the printing attribute
		''' values this Print Service supports for any possible job. If
		''' <CODE>flavor</CODE> is not null or <CODE>attributes</CODE> is not
		''' an empty set, this method returns just the printing attribute values
		''' that are compatible with the given doc flavor and/or set of attributes.
		''' That is, a null return value may indicate that specifying this attribute
		''' is incompatible with the specified DocFlavor.
		''' Also if DocFlavor is not null it must be a flavor supported by this
		''' PrintService, else IllegalArgumentException will be thrown.
		''' <P>
		''' If the <code>attributes</code> parameter contains an Attribute whose
		''' category is the same as the <code>category</code> parameter, the service
		''' must ignore this attribute in the AttributeSet.
		''' <p>
		''' <code>DocAttribute</code>s which are to be specified on the
		''' <code>Doc</code> must be included in this set to accurately
		''' represent the context.
		''' <p>
		''' This method returns an Object because different printing attribute
		''' categories indicate the supported attribute values in different ways.
		''' The documentation for each printing attribute in package {@link
		''' javax.print.attribute.standard javax.print.attribute.standard}
		''' describes how each attribute indicates its supported values. Possible
		''' ways of indicating support include:
		''' <UL>
		''' <LI>
		''' Return a single instance of the attribute category to indicate that any
		''' value is legal -- used, for example, by an attribute whose value is an
		''' arbitrary text string. (The value of the returned attribute object is
		''' irrelevant.)
		''' <LI>
		''' Return an array of one or more instances of the attribute category,
		''' containing the legal values -- used, for example, by an attribute with
		''' a list of enumerated values. The type of the array is an array of the
		''' specified attribute category type as returned by its
		''' <code>getCategory(Class)</code>.
		''' <LI>
		''' Return a single object (of some class other than the attribute category)
		''' that indicates bounds on the legal values -- used, for example, by an
		''' integer-valued attribute that must lie within a certain range.
		''' </UL>
		''' <P>
		''' </summary>
		''' <param name="category">    Printing attribute category to test. It must be a
		'''                        <seealso cref="java.lang.Class Class"/> that implements
		'''                        interface {@link
		'''                        javax.print.attribute.Attribute Attribute}. </param>
		''' <param name="flavor">      Doc flavor for a supposed job, or null. </param>
		''' <param name="attributes">  Set of printing attributes for a supposed job
		'''                        (both job-level attributes and document-level
		'''                        attributes), or null.
		''' </param>
		''' <returns>  Object indicating supported values for <CODE>category</CODE>,
		'''          or null if this Print Service does not support specifying a
		'''          doc-level or job-level attribute in <CODE>category</CODE> in
		'''          a Print Request.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>category</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (unchecked exception) Thrown if <CODE>category</CODE> is not a
		'''     <seealso cref="java.lang.Class Class"/> that implements interface {@link
		'''     javax.print.attribute.Attribute Attribute}, or
		'''     <code>DocFlavor</code> is not supported by this service. </exception>
		Function getSupportedAttributeValues(ByVal category As Type, ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As Object

		''' <summary>
		''' Determines whether a client can specify the given printing
		''' attribute
		''' value when setting up a job for this Print Service. A printing
		''' attribute value is an instance of a class that implements interface
		'''  <seealso cref="javax.print.attribute.Attribute Attribute"/>.
		''' <P>
		''' If <CODE>flavor</CODE> is null and <CODE>attributes</CODE> is null or
		''' is an empty set, this method tells whether this Print Service supports
		''' the given printing attribute value for some possible combination of doc
		''' flavor and set of attributes. If <CODE>flavor</CODE> is not null or
		''' <CODE>attributes</CODE> is not an empty set, this method tells whether
		''' this Print Service supports the given printing attribute value in
		''' combination with the given doc flavor and/or set of attributes.
		''' <p>
		''' Also if DocFlavor is not null it must be a flavor supported by this
		''' PrintService, else IllegalArgumentException will be thrown.
		''' <p>
		''' <code>DocAttribute</code>s which are to be specified on the
		''' <code>Doc</code> must be included in this set to accurately
		''' represent the context.
		''' <p>
		''' This is a convenience method to determine if the value
		''' would be a member of the result of
		''' <code>getSupportedAttributeValues(...)</code>.
		''' </summary>
		''' <param name="attrval">       Printing attribute value to test. </param>
		''' <param name="flavor">      Doc flavor for a supposed job, or null. </param>
		''' <param name="attributes">  Set of printing attributes for a supposed job
		'''                        (both job-level attributes and document-level
		'''                        attributes), or null.
		''' </param>
		''' <returns>  True if this Print Service supports specifying
		'''        <CODE>attrval</CODE> as a doc-level or job-level attribute in a
		'''          Print Request, false if it doesn't.
		''' </returns>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception)  if <CODE>attrval</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if flavor is not supported by
		'''      this PrintService. </exception>
		Function isAttributeValueSupported(ByVal attrval As javax.print.attribute.Attribute, ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As Boolean


		''' <summary>
		''' Identifies the attributes that are unsupported for a print request
		''' in the context of a particular DocFlavor.
		''' This method is useful for validating a potential print job and
		''' identifying the specific attributes which cannot be supported.
		''' It is important to supply only a supported DocFlavor or an
		''' IllegalArgumentException will be thrown. If the
		''' return value from this method is null, all attributes are supported.
		''' <p>
		''' <code>DocAttribute</code>s which are to be specified on the
		''' <code>Doc</code> must be included in this set to accurately
		''' represent the context.
		''' <p>
		''' If the return value is non-null, all attributes in the returned
		''' set are unsupported with this DocFlavor. The returned set does not
		''' distinguish attribute categories that are unsupported from
		''' unsupported attribute values.
		''' <p>
		''' A supported print request can then be created by removing
		''' all unsupported attributes from the original attribute set,
		''' except in the case that the DocFlavor is unsupported.
		''' <p>
		''' If any attributes are unsupported only because they are in conflict
		''' with other attributes then it is at the discretion of the service
		''' to select the attribute(s) to be identified as the cause of the
		''' conflict.
		''' <p>
		''' Use <code>isDocFlavorSupported()</code> to verify that a DocFlavor
		''' is supported before calling this method.
		''' </summary>
		''' <param name="flavor">      Doc flavor to test, or null </param>
		''' <param name="attributes">  Set of printing attributes for a supposed job
		'''                        (both job-level attributes and document-level
		'''                        attributes), or null.
		''' </param>
		''' <returns>  null if this Print Service supports the print request
		''' specification, else the unsupported attributes.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if<CODE>flavor</CODE> is
		'''             not supported by this PrintService. </exception>
		Function getUnsupportedAttributes(ByVal flavor As DocFlavor, ByVal attributes As javax.print.attribute.AttributeSet) As javax.print.attribute.AttributeSet

		''' <summary>
		''' Returns a factory for UI components which allow users to interact
		''' with the service in various roles.
		''' Services which do not provide any UI should return null.
		''' Print Services which do provide UI but want to be supported in
		''' an environment with no UI support should ensure that the factory
		''' is not initialised unless the application calls this method to
		''' obtain the factory.
		''' See <code>ServiceUIFactory</code> for more information. </summary>
		''' <returns> null or a factory for UI components. </returns>
		ReadOnly Property serviceUIFactory As ServiceUIFactory

		''' <summary>
		''' Determines if two services are referring to the same underlying
		''' service.  Objects encapsulating a print service may not exhibit
		''' equality of reference even though they refer to the same underlying
		''' service.
		''' <p>
		''' Clients should call this method to determine if two services are
		''' referring to the same underlying service.
		''' <p>
		''' Services must implement this method and return true only if the
		''' service objects being compared may be used interchangeably by the
		''' client.
		''' Services are free to return the same object reference to an underlying
		''' service if that, but clients must not depend on equality of reference. </summary>
		''' <param name="obj"> the reference object with which to compare. </param>
		''' <returns> true if this service is the same as the obj argument,
		''' false otherwise. </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' This method should be implemented consistently with
		''' <code>equals(Object)</code>. </summary>
		''' <returns> hash code of this object. </returns>
		Function GetHashCode() As Integer

	End Interface

End Namespace