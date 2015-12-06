﻿using System;


namespace Nez
{
	/// <summary>
	/// utility class to assist with dealing with bitmasks
	/// </summary>
	public static class Flags
	{
		/// <summary>
		/// checks to see if the bit flag is set in the int
		/// </summary>
		/// <returns><c>true</c>, if flag set was ised, <c>false</c> otherwise.</returns>
		/// <param name="self">Self.</param>
		/// <param name="flag">Flag.</param>
		public static bool isFlagSet( this int self, int flag )
		{
			return ( self & flag ) != 0;
		}


		/// <summary>
		/// sets the flag bit of the int
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="flag">Flag.</param>
		public static void setFlag( ref int self, int flag )
		{
			self = ( self | flag );
		}


		/// <summary>
		/// unsets the flag bit of the int
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="flag">Flag.</param>
		public static void unsetFlag( ref int self, int flag )
		{
			self = ( self & ( ~flag ) );
		}


		/// <summary>
		/// inverts the set bits of the int
		/// </summary>
		/// <param name="self">Self.</param>
		public static void invertFlags( ref int self )
		{
			self = ~self;
		}


		/// <summary>
		/// prints the binary representation of the int. Handy for debugging int flag overlaps visually.
		/// </summary>
		/// <returns>The string representation.</returns>
		/// <param name="self">Self.</param>
		/// <param name="leftPadWidth">Left pad width.</param>
		public static string binaryStringRepresentation( this int self, int leftPadWidth = 10 )
		{
			return Convert.ToString( self, 2 ).PadLeft( leftPadWidth, '0' );
		}

	}
}
