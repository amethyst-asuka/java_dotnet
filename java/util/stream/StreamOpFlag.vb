'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util.stream


	''' <summary>
	''' Flags corresponding to characteristics of streams and operations. Flags are
	''' utilized by the stream framework to control, specialize or optimize
	''' computation.
	''' 
	''' <p>
	''' Stream flags may be used to describe characteristics of several different
	''' entities associated with streams: stream sources, intermediate operations,
	''' and terminal operations.  Not all stream flags are meaningful for all
	''' entities; the following table summarizes which flags are meaningful in what
	''' contexts:
	''' 
	''' <div>
	''' <table>
	'''   <caption>Type Characteristics</caption>
	'''   <thead class="tableSubHeadingColor">
	'''     <tr>
	'''       <th colspan="2">&nbsp;</th>
	'''       <th>{@code DISTINCT}</th>
	'''       <th>{@code SORTED}</th>
	'''       <th>{@code ORDERED}</th>
	'''       <th>{@code SIZED}</th>
	'''       <th>{@code SHORT_CIRCUIT}</th>
	'''     </tr>
	'''   </thead>
	'''   <tbody>
	'''      <tr>
	'''        <th colspan="2" class="tableSubHeadingColor">Stream source</th>
	'''        <td>Y</td>
	'''        <td>Y</td>
	'''        <td>Y</td>
	'''        <td>Y</td>
	'''        <td>N</td>
	'''      </tr>
	'''      <tr>
	'''        <th colspan="2" class="tableSubHeadingColor">Intermediate operation</th>
	'''        <td>PCI</td>
	'''        <td>PCI</td>
	'''        <td>PCI</td>
	'''        <td>PC</td>
	'''        <td>PI</td>
	'''      </tr>
	'''      <tr>
	'''        <th colspan="2" class="tableSubHeadingColor">Terminal operation</th>
	'''        <td>N</td>
	'''        <td>N</td>
	'''        <td>PC</td>
	'''        <td>N</td>
	'''        <td>PI</td>
	'''      </tr>
	'''   </tbody>
	'''   <tfoot>
	'''       <tr>
	'''         <th class="tableSubHeadingColor" colspan="2">Legend</th>
	'''         <th colspan="6" rowspan="7">&nbsp;</th>
	'''       </tr>
	'''       <tr>
	'''         <th class="tableSubHeadingColor">Flag</th>
	'''         <th class="tableSubHeadingColor">Meaning</th>
	'''         <th colspan="6"></th>
	'''       </tr>
	'''       <tr><td>Y</td><td>Allowed</td></tr>
	'''       <tr><td>N</td><td>Invalid</td></tr>
	'''       <tr><td>P</td><td>Preserves</td></tr>
	'''       <tr><td>C</td><td>Clears</td></tr>
	'''       <tr><td>I</td><td>Injects</td></tr>
	'''   </tfoot>
	''' </table>
	''' </div>
	''' 
	''' <p>In the above table, "PCI" means "may preserve, clear, or inject"; "PC"
	''' means "may preserve or clear", "PI" means "may preserve or inject", and "N"
	''' means "not valid".
	''' 
	''' <p>Stream flags are represented by unioned bit sets, so that a single word
	''' may describe all the characteristics of a given stream entity, and that, for
	''' example, the flags for a stream source can be efficiently combined with the
	''' flags for later operations on that stream.
	''' 
	''' <p>The bit masks <seealso cref="#STREAM_MASK"/>, <seealso cref="#OP_MASK"/>, and
	''' <seealso cref="#TERMINAL_OP_MASK"/> can be ANDed with a bit set of stream flags to
	''' produce a mask containing only the valid flags for that entity type.
	''' 
	''' <p>When describing a stream source, one only need describe what
	''' characteristics that stream has; when describing a stream operation, one need
	''' describe whether the operation preserves, injects, or clears that
	''' characteristic.  Accordingly, two bits are used for each flag, so as to allow
	''' representing not only the presence of of a characteristic, but how an
	''' operation modifies that characteristic.  There are two common forms in which
	''' flag bits are combined into an {@code int} bit set.  <em>Stream flags</em>
	''' are a unioned bit set constructed by ORing the enum characteristic values of
	''' <seealso cref="#set()"/> (or, more commonly, ORing the corresponding static named
	''' constants prefixed with {@code IS_}).  <em>Operation flags</em> are a unioned
	''' bit set constructed by ORing the enum characteristic values of <seealso cref="#set()"/>
	''' or <seealso cref="#clear()"/> (to inject, or clear, respectively, the corresponding
	''' flag), or more commonly ORing the corresponding named constants prefixed with
	''' {@code IS_} or {@code NOT_}.  Flags that are not marked with {@code IS_} or
	''' {@code NOT_} are implicitly treated as preserved.  Care must be taken when
	''' combining bitsets that the correct combining operations are applied in the
	''' correct order.
	''' 
	''' <p>
	''' With the exception of <seealso cref="#SHORT_CIRCUIT"/>, stream characteristics can be
	''' derived from the equivalent <seealso cref="java.util.Spliterator"/> characteristics:
	''' <seealso cref="java.util.Spliterator#DISTINCT"/>, <seealso cref="java.util.Spliterator#SORTED"/>,
	''' <seealso cref="java.util.Spliterator#ORDERED"/>, and
	''' <seealso cref="java.util.Spliterator#SIZED"/>.  A spliterator characteristics bit set
	''' can be converted to stream flags using the method
	''' <seealso cref="#fromCharacteristics(java.util.Spliterator)"/> and converted back using
	''' <seealso cref="#toCharacteristics(int)"/>.  (The bit set
	''' <seealso cref="#SPLITERATOR_CHARACTERISTICS_MASK"/> is used to AND with a bit set to
	''' produce a valid spliterator characteristics bit set that can be converted to
	''' stream flags.)
	''' 
	''' <p>
	''' The source of a stream encapsulates a spliterator. The characteristics of
	''' that source spliterator when transformed to stream flags will be a proper
	''' subset of stream flags of that stream.
	''' For example:
	''' <pre> {@code
	'''     Spliterator s = ...;
	'''     Stream stream = Streams.stream(s);
	'''     flagsFromSplitr = fromCharacteristics(s.characteristics());
	'''     assert(flagsFromSplitr & stream.getStreamFlags() == flagsFromSplitr);
	''' }</pre>
	''' 
	''' <p>
	''' An intermediate operation, performed on an input stream to create a new
	''' output stream, may preserve, clear or inject stream or operation
	''' characteristics.  Similarly, a terminal operation, performed on an input
	''' stream to produce an output result may preserve, clear or inject stream or
	''' operation characteristics.  Preservation means that if that characteristic
	''' is present on the input, then it is also present on the output.  Clearing
	''' means that the characteristic is not present on the output regardless of the
	''' input.  Injection means that the characteristic is present on the output
	''' regardless of the input.  If a characteristic is not cleared or injected then
	''' it is implicitly preserved.
	''' 
	''' <p>
	''' A pipeline consists of a stream source encapsulating a spliterator, one or
	''' more intermediate operations, and finally a terminal operation that produces
	''' a result.  At each stage of the pipeline, a combined stream and operation
	''' flags can be calculated, using <seealso cref="#combineOpFlags(int, int)"/>.  Such flags
	''' ensure that preservation, clearing and injecting information is retained at
	''' each stage.
	''' 
	''' The combined stream and operation flags for the source stage of the pipeline
	''' is calculated as follows:
	''' <pre> {@code
	'''     int flagsForSourceStage = combineOpFlags(sourceFlags, INITIAL_OPS_VALUE);
	''' }</pre>
	''' 
	''' The combined stream and operation flags of each subsequent intermediate
	''' operation stage in the pipeline is calculated as follows:
	''' <pre> {@code
	'''     int flagsForThisStage = combineOpFlags(flagsForPreviousStage, thisOpFlags);
	''' }</pre>
	''' 
	''' Finally the flags output from the last intermediate operation of the pipeline
	''' are combined with the operation flags of the terminal operation to produce
	''' the flags output from the pipeline.
	''' 
	''' <p>Those flags can then be used to apply optimizations. For example, if
	''' {@code SIZED.isKnown(flags)} returns true then the stream size remains
	''' constant throughout the pipeline, this information can be utilized to
	''' pre-allocate data structures and combined with
	''' <seealso cref="java.util.Spliterator#SUBSIZED"/> that information can be utilized to
	''' perform concurrent in-place updates into a shared array.
	''' 
	''' For specific details see the <seealso cref="AbstractPipeline"/> constructors.
	''' 
	''' @since 1.8
	''' </summary>
	Friend Enum StreamOpFlag

	'    
	'     * Each characteristic takes up 2 bits in a bit set to accommodate
	'     * preserving, clearing and setting/injecting information.
	'     *
	'     * This applies to stream flags, intermediate/terminal operation flags, and
	'     * combined stream and operation flags. Even though the former only requires
	'     * 1 bit of information per characteristic, is it more efficient when
	'     * combining flags to align set and inject bits.
	'     *
	'     * Characteristics belong to certain types, see the Type enum. Bit masks for
	'     * the types are constructed as per the following table:
	'     *
	'     *                        DISTINCT  SORTED  ORDERED  SIZED  SHORT_CIRCUIT
	'     *          SPLITERATOR      01       01       01      01        00
	'     *               STREAM      01       01       01      01        00
	'     *                   OP      11       11       11      10        01
	'     *          TERMINAL_OP      00       00       10      00        01
	'     * UPSTREAM_TERMINAL_OP      00       00       10      00        00
	'     *
	'     * 01 = set/inject
	'     * 10 = clear
	'     * 11 = preserve
	'     *
	'     * Construction of the columns is performed using a simple builder for
	'     * non-zero values.
	'     


		' The following flags correspond to characteristics on Spliterator
		' and the values MUST be equal.
		'

		''' <summary>
		''' Characteristic value signifying that, for each pair of
		''' encountered elements in a stream {@code x, y}, {@code !x.equals(y)}.
		''' <p>
		''' A stream may have this value or an intermediate operation can preserve,
		''' clear or inject this value.
		''' </summary>
		' 0, 0x00000001
		' Matches Spliterator.DISTINCT
		DISTINCT(0,
				 [set](Type.SPLITERATOR).set(Type.STREAM).setAndClear(Type.OP))

		''' <summary>
		''' Characteristic value signifying that encounter order follows a natural
		''' sort order of comparable elements.
		''' <p>
		''' A stream can have this value or an intermediate operation can preserve,
		''' clear or inject this value.
		''' <p>
		''' Note: The <seealso cref="java.util.Spliterator#SORTED"/> characteristic can define
		''' a sort order with an associated non-null comparator.  Augmenting flag
		''' state with addition properties such that those properties can be passed
		''' to operations requires some disruptive changes for a singular use-case.
		''' Furthermore, comparing comparators for equality beyond that of identity
		''' is likely to be unreliable.  Therefore the {@code SORTED} characteristic
		''' for a defined non-natural sort order is not mapped internally to the
		''' {@code SORTED} flag.
		''' </summary>
		' 1, 0x00000004
		' Matches Spliterator.SORTED
		SORTED(1,
			   [set](Type.SPLITERATOR).set(Type.STREAM).setAndClear(Type.OP))

		''' <summary>
		''' Characteristic value signifying that an encounter order is
		''' defined for stream elements.
		''' <p>
		''' A stream can have this value, an intermediate operation can preserve,
		''' clear or inject this value, or a terminal operation can preserve or clear
		''' this value.
		''' </summary>
		' 2, 0x00000010
		' Matches Spliterator.ORDERED
		ORDERED(2,
				[set](Type.SPLITERATOR).set(Type.STREAM).setAndClear(Type.OP).clear(Type.TERMINAL_OP).clear(Type.UPSTREAM_TERMINAL_OP))

		''' <summary>
		''' Characteristic value signifying that size of the stream
		''' is of a known finite size that is equal to the known finite
		''' size of the source spliterator input to the first stream
		''' in the pipeline.
		''' <p>
		''' A stream can have this value or an intermediate operation can preserve or
		''' clear this value.
		''' </summary>
		' 3, 0x00000040
		' Matches Spliterator.SIZED
		SIZED(3,
			  [set](Type.SPLITERATOR).set(Type.STREAM).clear(Type.OP))

		' The following Spliterator characteristics are not currently used but a
		' gap in the bit set is deliberately retained to enable corresponding
		' stream flags if//when required without modification to other flag values.
		'
		' 4, 0x00000100 NONNULL(4, ...
		' 5, 0x00000400 IMMUTABLE(5, ...
		' 6, 0x00001000 CONCURRENT(6, ...
		' 7, 0x00004000 SUBSIZED(7, ...

		' The following 4 flags are currently undefined and a free for any further
		' spliterator characteristics.
		'
		'  8, 0x00010000
		'  9, 0x00040000
		' 10, 0x00100000
		' 11, 0x00400000

		' The following flags are specific to streams and operations
		'

		''' <summary>
		''' Characteristic value signifying that an operation may short-circuit the
		''' stream.
		''' <p>
		''' An intermediate operation can preserve or inject this value,
		''' or a terminal operation can preserve or inject this value.
		''' </summary>
		' 12, 0x01000000
		SHORT_CIRCUIT(12,
					  [set](Type.OP).set(Type.TERMINAL_OP))

		' The following 2 flags are currently undefined and a free for any further
		' stream flags if/when required
		'
		' 13, 0x04000000
		' 14, 0x10000000
		' 15, 0x40000000

		''' <summary>
		''' Type of a flag
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		enum Type
			''' <summary>
			''' The flag is associated with spliterator characteristics.
			''' </summary>
			SPLITERATOR

			''' <summary>
			''' The flag is associated with stream flags.
			''' </summary>
			STREAM

			''' <summary>
			''' The flag is associated with intermediate operation flags.
			''' </summary>
			OP

			''' <summary>
			''' The flag is associated with terminal operation flags.
			''' </summary>
			TERMINAL_OP

			''' <summary>
			''' The flag is associated with terminal operation flags that are
			''' propagated upstream across the last stateful operation boundary
			''' </summary>
			UPSTREAM_TERMINAL_OP

		''' <summary>
		''' The bit pattern for setting/injecting a flag.
		''' </summary>
	'JAVA TO VB CONVERTER TODO TASK: Binary literals are not available in VB:
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final int SET_BITS = 0b01;

		''' <summary>
		''' The bit pattern for clearing a flag.
		''' </summary>
	'JAVA TO VB CONVERTER TODO TASK: Binary literals are not available in VB:
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final int CLEAR_BITS = 0b10;

		''' <summary>
		''' The bit pattern for preserving a flag.
		''' </summary>
	'JAVA TO VB CONVERTER TODO TASK: Binary literals are not available in VB:
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final int PRESERVE_BITS = 0b11;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static MaskBuilder set(Type t)
	'	{
	'		Return New MaskBuilder(New EnumMap<>(Type.class)).set(t);
	'	}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private static class MaskBuilder
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			final java.util.Map(Of Type, java.lang.Integer) map;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			MaskBuilder(java.util.Map(Of Type, java.lang.Integer) map)
	'		{
	'			Me.map = map;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			MaskBuilder mask(Type t, java.lang.Integer i)
	'		{
	'			map.put(t, i);
	'			Return Me;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			MaskBuilder set(Type t)
	'		{
	'			Return mask(t, SET_BITS);
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			MaskBuilder clear(Type t)
	'		{
	'			Return mask(t, CLEAR_BITS);
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			MaskBuilder setAndClear(Type t)
	'		{
	'			Return mask(t, PRESERVE_BITS);
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			java.util.Map(Of Type, java.lang.Integer) build()
	'		{
	'			for (Type t : Type.values())
	'			{
	''JAVA TO VB CONVERTER TODO TASK: Binary literals are not available in VB:
	'				map.putIfAbsent(t, 0b00);
	'			}
	'			Return map;
	'		}

		''' <summary>
		''' The mask table for a flag, this is used to determine if a flag
		''' corresponds to a certain flag type and for creating mask constants.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final java.util.Map(Of Type, java.lang.Integer) maskTable;

		''' <summary>
		''' The bit position in the bit mask.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final int bitPosition;

		''' <summary>
		''' The set 2 bit set offset at the bit position.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final int set;

		''' <summary>
		''' The clear 2 bit set offset at the bit position.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final int clear;

		''' <summary>
		''' The preserve 2 bit set offset at the bit position.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final int preserve;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private StreamOpFlag(int position, MaskBuilder maskBuilder)
	'	{
	'		Me.maskTable = maskBuilder.build();
	'		' Two bits per flag
	'		position *= 2;
	'		Me.bitPosition = position;
	'		Me.set = SET_BITS << position;
	'		Me.clear = CLEAR_BITS << position;
	'		Me.preserve = PRESERVE_BITS << position;
	'	}

		''' <summary>
		''' Gets the bitmap associated with setting this characteristic.
		''' </summary>
		''' <returns> the bitmap for setting this characteristic </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		int set()
	'	{
	'		Return set;
	'	}

		''' <summary>
		''' Gets the bitmap associated with clearing this characteristic.
		''' </summary>
		''' <returns> the bitmap for clearing this characteristic </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		int clear()
	'	{
	'		Return clear;
	'	}

		''' <summary>
		''' Determines if this flag is a stream-based flag.
		''' </summary>
		''' <returns> true if a stream-based flag, otherwise false. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		boolean isStreamFlag()
	'	{
	'		Return maskTable.get(Type.STREAM) > 0;
	'	}

		''' <summary>
		''' Checks if this flag is set on stream flags, injected on operation flags,
		''' and injected on combined stream and operation flags.
		''' </summary>
		''' <param name="flags"> the stream flags, operation flags, or combined stream and
		'''        operation flags </param>
		''' <returns> true if this flag is known, otherwise false. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		boolean isKnown(int flags)
	'	{
	'		Return (flags & preserve) == set;
	'	}

		''' <summary>
		''' Checks if this flag is cleared on operation flags or combined stream and
		''' operation flags.
		''' </summary>
		''' <param name="flags"> the operation flags or combined stream and operations flags. </param>
		''' <returns> true if this flag is preserved, otherwise false. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		boolean isCleared(int flags)
	'	{
	'		Return (flags & preserve) == clear;
	'	}

		''' <summary>
		''' Checks if this flag is preserved on combined stream and operation flags.
		''' </summary>
		''' <param name="flags"> the combined stream and operations flags. </param>
		''' <returns> true if this flag is preserved, otherwise false. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		boolean isPreserved(int flags)
	'	{
	'		Return (flags & preserve) == preserve;
	'	}

		''' <summary>
		''' Determines if this flag can be set for a flag type.
		''' </summary>
		''' <param name="t"> the flag type. </param>
		''' <returns> true if this flag can be set for the flag type, otherwise false. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		boolean canSet(Type t)
	'	{
	'		Return (maskTable.get(t) & SET_BITS) > 0;
	'	}

		''' <summary>
		''' The bit mask for spliterator characteristics
		''' </summary>
		[static] = Type.SPLITERATOR

		''' <summary>
		''' The bit mask for source stream flags.
		''' </summary>
		[static] = Type.STREAM

		''' <summary>
		''' The bit mask for intermediate operation flags.
		''' </summary>
		[static] = Type.OP

		''' <summary>
		''' The bit mask for terminal operation flags.
		''' </summary>
		[static] = Type.TERMINAL_OP

		''' <summary>
		''' The bit mask for upstream terminal operation flags.
		''' </summary>
		[static] = Type.UPSTREAM_TERMINAL_OP

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static int createMask(Type t)
	'	{
	'		int mask = 0;
	'		for (StreamOpFlag flag : StreamOpFlag.values())
	'		{
	'			mask |= flag.maskTable.get(t) << flag.bitPosition;
	'		}
	'		Return mask;
	'	}

		''' <summary>
		''' Complete flag mask.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final int FLAG_MASK = createFlagMask();

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static int createFlagMask()
	'	{
	'		int mask = 0;
	'		for (StreamOpFlag flag : StreamOpFlag.values())
	'		{
	'			mask |= flag.preserve;
	'		}
	'		Return mask;
	'	}

		''' <summary>
		''' Flag mask for stream flags that are set.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final int FLAG_MASK_IS = STREAM_MASK;

		''' <summary>
		''' Flag mask for stream flags that are cleared.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static final int FLAG_MASK_NOT = STREAM_MASK << 1;

		''' <summary>
		''' The initial value to be combined with the stream flags of the first
		''' stream in the pipeline.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int INITIAL_OPS_VALUE = FLAG_MASK_IS | FLAG_MASK_NOT;

		''' <summary>
		''' The bit value to set or inject <seealso cref="#DISTINCT"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int IS_DISTINCT = DISTINCT.set;

		''' <summary>
		''' The bit value to clear <seealso cref="#DISTINCT"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int NOT_DISTINCT = DISTINCT.clear;

		''' <summary>
		''' The bit value to set or inject <seealso cref="#SORTED"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int IS_SORTED = SORTED.set;

		''' <summary>
		''' The bit value to clear <seealso cref="#SORTED"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int NOT_SORTED = SORTED.clear;

		''' <summary>
		''' The bit value to set or inject <seealso cref="#ORDERED"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int IS_ORDERED = ORDERED.set;

		''' <summary>
		''' The bit value to clear <seealso cref="#ORDERED"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int NOT_ORDERED = ORDERED.clear;

		''' <summary>
		''' The bit value to set <seealso cref="#SIZED"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int IS_SIZED = SIZED.set;

		''' <summary>
		''' The bit value to clear <seealso cref="#SIZED"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int NOT_SIZED = SIZED.clear;

		''' <summary>
		''' The bit value to inject <seealso cref="#SHORT_CIRCUIT"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		static final int IS_SHORT_CIRCUIT = SHORT_CIRCUIT.set;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static int getMask(int flags)
	'	{
	'		Return (flags == 0) ? FLAG_MASK : ~(flags | ((FLAG_MASK_IS & flags) << 1) | ((FLAG_MASK_NOT & flags) >> 1));
	'	}

		''' <summary>
		''' Combines stream or operation flags with previously combined stream and
		''' operation flags to produce updated combined stream and operation flags.
		''' <p>
		''' A flag set on stream flags or injected on operation flags,
		''' and injected combined stream and operation flags,
		''' will be injected on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag set on stream flags or injected on operation flags,
		''' and cleared on the combined stream and operation flags,
		''' will be cleared on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag set on the stream flags or injected on operation flags,
		''' and preserved on the combined stream and operation flags,
		''' will be injected on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag not set on the stream flags or cleared/preserved on operation
		''' flags, and injected on the combined stream and operation flags,
		''' will be injected on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag not set on the stream flags or cleared/preserved on operation
		''' flags, and cleared on the combined stream and operation flags,
		''' will be cleared on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag not set on the stream flags,
		''' and preserved on the combined stream and operation flags
		''' will be preserved on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag cleared on operation flags,
		''' and preserved on the combined stream and operation flags
		''' will be cleared on the updated combined stream and operation flags.
		''' 
		''' <p>
		''' A flag preserved on operation flags,
		''' and preserved on the combined stream and operation flags
		''' will be preserved on the updated combined stream and operation flags.
		''' </summary>
		''' <param name="newStreamOrOpFlags"> the stream or operation flags. </param>
		''' <param name="prevCombOpFlags"> previously combined stream and operation flags.
		'''        The value {#link INITIAL_OPS_VALUE} must be used as the seed value. </param>
		''' <returns> the updated combined stream and operation flags. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static int combineOpFlags(int newStreamOrOpFlags, int prevCombOpFlags)
	'	{
	'		' 0x01 or 0x10 nibbles are transformed to 0x11
	'		' 0x00 nibbles remain unchanged
	'		' Then all the bits are flipped
	'		' Then the result is logically or'ed with the operation flags.
	'		Return (prevCombOpFlags & StreamOpFlag.getMask(newStreamOrOpFlags)) | newStreamOrOpFlags;
	'	}

		''' <summary>
		''' Converts combined stream and operation flags to stream flags.
		''' 
		''' <p>Each flag injected on the combined stream and operation flags will be
		''' set on the stream flags.
		''' </summary>
		''' <param name="combOpFlags"> the combined stream and operation flags. </param>
		''' <returns> the stream flags. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static int toStreamFlags(int combOpFlags)
	'	{
	'		' By flipping the nibbles 0x11 become 0x00 and 0x01 become 0x10
	'		' Shift left 1 to restore set flags and mask off anything other than the set flags
	'		Return ((~combOpFlags) >> 1) & FLAG_MASK_IS & combOpFlags;
	'	}

		''' <summary>
		''' Converts stream flags to a spliterator characteristic bit set.
		''' </summary>
		''' <param name="streamFlags"> the stream flags. </param>
		''' <returns> the spliterator characteristic bit set. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static int toCharacteristics(int streamFlags)
	'	{
	'		Return streamFlags & SPLITERATOR_CHARACTERISTICS_MASK;
	'	}

		''' <summary>
		''' Converts a spliterator characteristic bit set to stream flags.
		''' 
		''' @implSpec
		''' If the spliterator is naturally {@code SORTED} (the associated
		''' {@code Comparator} is {@code null}) then the characteristic is converted
		''' to the <seealso cref="#SORTED"/> flag, otherwise the characteristic is not
		''' converted.
		''' </summary>
		''' <param name="spliterator"> the spliterator from which to obtain characteristic
		'''        bit set. </param>
		''' <returns> the stream flags. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static int fromCharacteristics(java.util.Spliterator(Of JavaToDotNetGenericWildcard) spliterator)
	'	{
	'		int characteristics = spliterator.characteristics();
	'		if ((characteristics & Spliterator.SORTED) != 0 && spliterator.getComparator() != Nothing)
	'		{
	'			' Do not propagate the SORTED characteristic if it does not correspond
	'			' to a natural sort order
	'			Return characteristics & SPLITERATOR_CHARACTERISTICS_MASK & ~Spliterator.SORTED;
	'		}
	'		else
	'		{
	'			Return characteristics & SPLITERATOR_CHARACTERISTICS_MASK;
	'		}
	'	}

		''' <summary>
		''' Converts a spliterator characteristic bit set to stream flags.
		''' </summary>
		''' <param name="characteristics"> the spliterator characteristic bit set. </param>
		''' <returns> the stream flags. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static int fromCharacteristics(int characteristics)
	'	{
	'		Return characteristics & SPLITERATOR_CHARACTERISTICS_MASK;
	'	}
	End Enum

End Namespace