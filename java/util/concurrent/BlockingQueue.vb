Imports System.Collections.Generic

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

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent


	''' <summary>
	''' A <seealso cref="java.util.Queue"/> that additionally supports operations
	''' that wait for the queue to become non-empty when retrieving an
	''' element, and wait for space to become available in the queue when
	''' storing an element.
	''' 
	''' <p>{@code BlockingQueue} methods come in four forms, with different ways
	''' of handling operations that cannot be satisfied immediately, but may be
	''' satisfied at some point in the future:
	''' one throws an exception, the second returns a special value (either
	''' {@code null} or {@code false}, depending on the operation), the third
	''' blocks the current thread indefinitely until the operation can succeed,
	''' and the fourth blocks for only a given maximum time limit before giving
	''' up.  These methods are summarized in the following table:
	''' 
	''' <table BORDER CELLPADDING=3 CELLSPACING=1>
	''' <caption>Summary of BlockingQueue methods</caption>
	'''  <tr>
	'''    <td></td>
	'''    <td ALIGN=CENTER><em>Throws exception</em></td>
	'''    <td ALIGN=CENTER><em>Special value</em></td>
	'''    <td ALIGN=CENTER><em>Blocks</em></td>
	'''    <td ALIGN=CENTER><em>Times out</em></td>
	'''  </tr>
	'''  <tr>
	'''    <td><b>Insert</b></td>
	'''    <td><seealso cref="#add add(e)"/></td>
	'''    <td><seealso cref="#offer offer(e)"/></td>
	'''    <td><seealso cref="#put put(e)"/></td>
	'''    <td><seealso cref="#offer(Object, long, TimeUnit) offer(e, time, unit)"/></td>
	'''  </tr>
	'''  <tr>
	'''    <td><b>Remove</b></td>
	'''    <td><seealso cref="#remove remove()"/></td>
	'''    <td><seealso cref="#poll poll()"/></td>
	'''    <td><seealso cref="#take take()"/></td>
	'''    <td><seealso cref="#poll(long, TimeUnit) poll(time, unit)"/></td>
	'''  </tr>
	'''  <tr>
	'''    <td><b>Examine</b></td>
	'''    <td><seealso cref="#element element()"/></td>
	'''    <td><seealso cref="#peek peek()"/></td>
	'''    <td><em>not applicable</em></td>
	'''    <td><em>not applicable</em></td>
	'''  </tr>
	''' </table>
	''' 
	''' <p>A {@code BlockingQueue} does not accept {@code null} elements.
	''' Implementations throw {@code NullPointerException} on attempts
	''' to {@code add}, {@code put} or {@code offer} a {@code null}.  A
	''' {@code null} is used as a sentinel value to indicate failure of
	''' {@code poll} operations.
	''' 
	''' <p>A {@code BlockingQueue} may be capacity bounded. At any given
	''' time it may have a {@code remainingCapacity} beyond which no
	''' additional elements can be {@code put} without blocking.
	''' A {@code BlockingQueue} without any intrinsic capacity constraints always
	''' reports a remaining capacity of {@code  java.lang.[Integer].MAX_VALUE}.
	''' 
	''' <p>{@code BlockingQueue} implementations are designed to be used
	''' primarily for producer-consumer queues, but additionally support
	''' the <seealso cref="java.util.Collection"/> interface.  So, for example, it is
	''' possible to remove an arbitrary element from a queue using
	''' {@code remove(x)}. However, such operations are in general
	''' <em>not</em> performed very efficiently, and are intended for only
	''' occasional use, such as when a queued message is cancelled.
	''' 
	''' <p>{@code BlockingQueue} implementations are thread-safe.  All
	''' queuing methods achieve their effects atomically using internal
	''' locks or other forms of concurrency control. However, the
	''' <em>bulk</em> Collection operations {@code addAll},
	''' {@code containsAll}, {@code retainAll} and {@code removeAll} are
	''' <em>not</em> necessarily performed atomically unless specified
	''' otherwise in an implementation. So it is possible, for example, for
	''' {@code addAll(c)} to fail (throwing an exception) after adding
	''' only some of the elements in {@code c}.
	''' 
	''' <p>A {@code BlockingQueue} does <em>not</em> intrinsically support
	''' any kind of &quot;close&quot; or &quot;shutdown&quot; operation to
	''' indicate that no more items will be added.  The needs and usage of
	''' such features tend to be implementation-dependent. For example, a
	''' common tactic is for producers to insert special
	''' <em>end-of-stream</em> or <em>poison</em> objects, that are
	''' interpreted accordingly when taken by consumers.
	''' 
	''' <p>
	''' Usage example, based on a typical producer-consumer scenario.
	''' Note that a {@code BlockingQueue} can safely be used with multiple
	''' producers and multiple consumers.
	'''  <pre> {@code
	''' class Producer implements Runnable {
	'''   private final BlockingQueue queue;
	'''   Producer(BlockingQueue q) { queue = q; }
	'''   public  Sub  run() {
	'''     try {
	'''       while (true) { queue.put(produce()); }
	'''     } catch (InterruptedException ex) { ... handle ...}
	'''   }
	'''   Object produce() { ... }
	''' }
	''' 
	''' class Consumer implements Runnable {
	'''   private final BlockingQueue queue;
	'''   Consumer(BlockingQueue q) { queue = q; }
	'''   public  Sub  run() {
	'''     try {
	'''       while (true) { consume(queue.take()); }
	'''     } catch (InterruptedException ex) { ... handle ...}
	'''   }
	'''    Sub  consume(Object x) { ... }
	''' }
	''' 
	''' class Setup {
	'''    Sub  main() {
	'''     BlockingQueue q = new SomeQueueImplementation();
	'''     Producer p = new Producer(q);
	'''     Consumer c1 = new Consumer(q);
	'''     Consumer c2 = new Consumer(q);
	'''     new Thread(p).start();
	'''     new Thread(c1).start();
	'''     new Thread(c2).start();
	'''   }
	''' }}</pre>
	''' 
	''' <p>Memory consistency effects: As with other concurrent
	''' collections, actions in a thread prior to placing an object into a
	''' {@code BlockingQueue}
	''' <a href="package-summary.html#MemoryVisibility"><i>happen-before</i></a>
	''' actions subsequent to the access or removal of that element from
	''' the {@code BlockingQueue} in another thread.
	''' 
	''' <p>This interface is a member of the
	''' <a href="{@docRoot}/../technotes/guides/collections/index.html">
	''' Java Collections Framework</a>.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> the type of elements held in this collection </param>
	Public Interface BlockingQueue(Of E)
		Inherits LinkedList(Of E)

		''' <summary>
		''' Inserts the specified element into this queue if it is possible to do
		''' so immediately without violating capacity restrictions, returning
		''' {@code true} upon success and throwing an
		''' {@code IllegalStateException} if no space is currently available.
		''' When using a capacity-restricted queue, it is generally preferable to
		''' use <seealso cref="#offer(Object) offer"/>.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} (as specified by <seealso cref="Collection#add"/>) </returns>
		''' <exception cref="IllegalStateException"> if the element cannot be added at this
		'''         time due to capacity restrictions </exception>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         prevents it from being added to this queue </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		''' <exception cref="IllegalArgumentException"> if some property of the specified
		'''         element prevents it from being added to this queue </exception>
		Function add(ByVal e As E) As Boolean

		''' <summary>
		''' Inserts the specified element into this queue if it is possible to do
		''' so immediately without violating capacity restrictions, returning
		''' {@code true} upon success and {@code false} if no space is currently
		''' available.  When using a capacity-restricted queue, this method is
		''' generally preferable to <seealso cref="#add"/>, which can fail to insert an
		''' element only by throwing an exception.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <returns> {@code true} if the element was added to this queue, else
		'''         {@code false} </returns>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         prevents it from being added to this queue </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		''' <exception cref="IllegalArgumentException"> if some property of the specified
		'''         element prevents it from being added to this queue </exception>
		Function offer(ByVal e As E) As Boolean

		''' <summary>
		''' Inserts the specified element into this queue, waiting if necessary
		''' for space to become available.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         prevents it from being added to this queue </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		''' <exception cref="IllegalArgumentException"> if some property of the specified
		'''         element prevents it from being added to this queue </exception>
		Sub put(ByVal e As E)

		''' <summary>
		''' Inserts the specified element into this queue, waiting up to the
		''' specified wait time if necessary for space to become available.
		''' </summary>
		''' <param name="e"> the element to add </param>
		''' <param name="timeout"> how long to wait before giving up, in units of
		'''        {@code unit} </param>
		''' <param name="unit"> a {@code TimeUnit} determining how to interpret the
		'''        {@code timeout} parameter </param>
		''' <returns> {@code true} if successful, or {@code false} if
		'''         the specified waiting time elapses before space is available </returns>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         prevents it from being added to this queue </exception>
		''' <exception cref="NullPointerException"> if the specified element is null </exception>
		''' <exception cref="IllegalArgumentException"> if some property of the specified
		'''         element prevents it from being added to this queue </exception>
		Function offer(ByVal e As E, ByVal timeout As Long, ByVal unit As TimeUnit) As Boolean

		''' <summary>
		''' Retrieves and removes the head of this queue, waiting if necessary
		''' until an element becomes available.
		''' </summary>
		''' <returns> the head of this queue </returns>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
		Function take() As E

		''' <summary>
		''' Retrieves and removes the head of this queue, waiting up to the
		''' specified wait time if necessary for an element to become available.
		''' </summary>
		''' <param name="timeout"> how long to wait before giving up, in units of
		'''        {@code unit} </param>
		''' <param name="unit"> a {@code TimeUnit} determining how to interpret the
		'''        {@code timeout} parameter </param>
		''' <returns> the head of this queue, or {@code null} if the
		'''         specified waiting time elapses before an element is available </returns>
		''' <exception cref="InterruptedException"> if interrupted while waiting </exception>
		Function poll(ByVal timeout As Long, ByVal unit As TimeUnit) As E

		''' <summary>
		''' Returns the number of additional elements that this queue can ideally
		''' (in the absence of memory or resource constraints) accept without
		''' blocking, or {@code  java.lang.[Integer].MAX_VALUE} if there is no intrinsic
		''' limit.
		''' 
		''' <p>Note that you <em>cannot</em> always tell if an attempt to insert
		''' an element will succeed by inspecting {@code remainingCapacity}
		''' because it may be the case that another thread is about to
		''' insert or remove an element.
		''' </summary>
		''' <returns> the remaining capacity </returns>
		Function remainingCapacity() As Integer

		''' <summary>
		''' Removes a single instance of the specified element from this queue,
		''' if it is present.  More formally, removes an element {@code e} such
		''' that {@code o.equals(e)}, if this queue contains one or more such
		''' elements.
		''' Returns {@code true} if this queue contained the specified element
		''' (or equivalently, if this queue changed as a result of the call).
		''' </summary>
		''' <param name="o"> element to be removed from this queue, if present </param>
		''' <returns> {@code true} if this queue changed as a result of the call </returns>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         is incompatible with this queue
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		Function remove(ByVal o As Object) As Boolean

		''' <summary>
		''' Returns {@code true} if this queue contains the specified element.
		''' More formally, returns {@code true} if and only if this queue contains
		''' at least one element {@code e} such that {@code o.equals(e)}.
		''' </summary>
		''' <param name="o"> object to be checked for containment in this queue </param>
		''' <returns> {@code true} if this queue contains the specified element </returns>
		''' <exception cref="ClassCastException"> if the class of the specified element
		'''         is incompatible with this queue
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		''' <exception cref="NullPointerException"> if the specified element is null
		'''         (<a href="../Collection.html#optional-restrictions">optional</a>) </exception>
		Function contains(ByVal o As Object) As Boolean

		''' <summary>
		''' Removes all available elements from this queue and adds them
		''' to the given collection.  This operation may be more
		''' efficient than repeatedly polling this queue.  A failure
		''' encountered while attempting to add elements to
		''' collection {@code c} may result in elements being in neither,
		''' either or both collections when the associated exception is
		''' thrown.  Attempts to drain a queue to itself result in
		''' {@code IllegalArgumentException}. Further, the behavior of
		''' this operation is undefined if the specified collection is
		''' modified while the operation is in progress.
		''' </summary>
		''' <param name="c"> the collection to transfer elements into </param>
		''' <returns> the number of elements transferred </returns>
		''' <exception cref="UnsupportedOperationException"> if addition of elements
		'''         is not supported by the specified collection </exception>
		''' <exception cref="ClassCastException"> if the class of an element of this queue
		'''         prevents it from being added to the specified collection </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <exception cref="IllegalArgumentException"> if the specified collection is this
		'''         queue, or some property of an element of this queue prevents
		'''         it from being added to the specified collection </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Function drainTo(Of T1)(ByVal c As ICollection(Of T1)) As Integer

		''' <summary>
		''' Removes at most the given number of available elements from
		''' this queue and adds them to the given collection.  A failure
		''' encountered while attempting to add elements to
		''' collection {@code c} may result in elements being in neither,
		''' either or both collections when the associated exception is
		''' thrown.  Attempts to drain a queue to itself result in
		''' {@code IllegalArgumentException}. Further, the behavior of
		''' this operation is undefined if the specified collection is
		''' modified while the operation is in progress.
		''' </summary>
		''' <param name="c"> the collection to transfer elements into </param>
		''' <param name="maxElements"> the maximum number of elements to transfer </param>
		''' <returns> the number of elements transferred </returns>
		''' <exception cref="UnsupportedOperationException"> if addition of elements
		'''         is not supported by the specified collection </exception>
		''' <exception cref="ClassCastException"> if the class of an element of this queue
		'''         prevents it from being added to the specified collection </exception>
		''' <exception cref="NullPointerException"> if the specified collection is null </exception>
		''' <exception cref="IllegalArgumentException"> if the specified collection is this
		'''         queue, or some property of an element of this queue prevents
		'''         it from being added to the specified collection </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Function drainTo(Of T1)(ByVal c As ICollection(Of T1), ByVal maxElements As Integer) As Integer
	End Interface

End Namespace