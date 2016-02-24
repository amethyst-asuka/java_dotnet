'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	''' <summary>
	'''   <p>An annotation on a constructor that shows how the parameters of
	'''   that constructor correspond to the constructed object's getter
	'''   methods.  For example:
	''' 
	'''   <blockquote>
	''' <pre>
	'''   public class Point {
	'''       &#64;ConstructorProperties({"x", "y"})
	'''       public Point(int x, int y) {
	'''           this.x = x;
	'''           this.y = y;
	'''       }
	''' 
	'''       public int getX() {
	'''           return x;
	'''       }
	''' 
	'''       public int getY() {
	'''           return y;
	'''       }
	''' 
	'''       private final int x, y;
	'''   }
	''' </pre>
	''' </blockquote>
	''' 
	'''   The annotation shows that the first parameter of the constructor
	'''   can be retrieved with the {@code getX()} method and the second with
	'''   the {@code getY()} method.  Since parameter names are not in
	'''   general available at runtime, without the annotation there would be
	'''   no way to know whether the parameters correspond to {@code getX()}
	'''   and {@code getY()} or the other way around.
	''' 
	'''   @since 1.6
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to CONSTRUCTOR:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class ConstructorProperties
		Inherits System.Attribute

		''' <summary>
		'''   <p>The getter names.</p> </summary>
		'''   <returns> the getter names corresponding to the parameters in the
		'''   annotated constructor. </returns>
		String() value()
	End Class

End Namespace