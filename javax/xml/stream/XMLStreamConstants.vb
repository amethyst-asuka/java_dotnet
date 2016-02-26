'
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

Namespace javax.xml.stream

	''' <summary>
	''' This interface declares the constants used in this API.
	''' Numbers in the range 0 to 256 are reserved for the specification,
	''' user defined events must use event codes outside that range.
	''' 
	''' @since 1.6
	''' </summary>

	Public Interface XMLStreamConstants
	  ''' <summary>
	  ''' Indicates an event is a start element </summary>
	  ''' <seealso cref= javax.xml.stream.events.StartElement </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int START_ELEMENT=1;
	  ''' <summary>
	  ''' Indicates an event is an end element </summary>
	  ''' <seealso cref= javax.xml.stream.events.EndElement </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int END_ELEMENT=2;
	  ''' <summary>
	  ''' Indicates an event is a processing instruction </summary>
	  ''' <seealso cref= javax.xml.stream.events.ProcessingInstruction </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int PROCESSING_INSTRUCTION=3;

	  ''' <summary>
	  ''' Indicates an event is characters </summary>
	  ''' <seealso cref= javax.xml.stream.events.Characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int CHARACTERS=4;

	  ''' <summary>
	  ''' Indicates an event is a comment </summary>
	  ''' <seealso cref= javax.xml.stream.events.Comment </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int COMMENT=5;

	  ''' <summary>
	  ''' The characters are white space
	  ''' (see [XML], 2.10 "White Space Handling").
	  ''' Events are only reported as SPACE if they are ignorable white
	  ''' space.  Otherwise they are reported as CHARACTERS. </summary>
	  ''' <seealso cref= javax.xml.stream.events.Characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int SPACE=6;

	  ''' <summary>
	  ''' Indicates an event is a start document </summary>
	  ''' <seealso cref= javax.xml.stream.events.StartDocument </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int START_DOCUMENT=7;

	  ''' <summary>
	  ''' Indicates an event is an end document </summary>
	  ''' <seealso cref= javax.xml.stream.events.EndDocument </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int END_DOCUMENT=8;

	  ''' <summary>
	  ''' Indicates an event is an entity reference </summary>
	  ''' <seealso cref= javax.xml.stream.events.EntityReference </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int ENTITY_REFERENCE=9;

	  ''' <summary>
	  ''' Indicates an event is an attribute </summary>
	  ''' <seealso cref= javax.xml.stream.events.Attribute </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int ATTRIBUTE=10;

	  ''' <summary>
	  ''' Indicates an event is a DTD </summary>
	  ''' <seealso cref= javax.xml.stream.events.DTD </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int DTD=11;

	  ''' <summary>
	  ''' Indicates an event is a CDATA section </summary>
	  ''' <seealso cref= javax.xml.stream.events.Characters </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int CDATA=12;

	  ''' <summary>
	  ''' Indicates the event is a namespace declaration
	  ''' </summary>
	  ''' <seealso cref= javax.xml.stream.events.Namespace </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int NAMESPACE=13;

	  ''' <summary>
	  ''' Indicates a Notation </summary>
	  ''' <seealso cref= javax.xml.stream.events.NotationDeclaration </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int NOTATION_DECLARATION=14;

	  ''' <summary>
	  ''' Indicates a Entity Declaration </summary>
	  ''' <seealso cref= javax.xml.stream.events.NotationDeclaration </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final int ENTITY_DECLARATION=15;
	End Interface

End Namespace