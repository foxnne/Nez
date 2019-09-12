using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Nez
{
    [Serializable]
    [DebuggerDisplay("{DebugDisplayString, nq}")]
    public struct Direction : IComparable<byte>, IEquatable<Direction>, IFormattable, IEquatable<byte>
    {
        /// <summary>Wrapped byte representing eight Directions rotating anti-clockwise starting with south (zero).</summary>
        [Range(0, 7, 1)]
        public byte Value;

        /// <summary>Creates a Direction with value equal to i.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(int i = 0) { Value = (byte)Mathf.Wrap(i, 8); }

        /// <summary>Creates a Direction with value equal to b % 8.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(byte b) { Value = (byte)Mathf.Wrap(b, 8); }

        /// <summary>Creates a Direction with value equal to rounded f % 8.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(float f) { Value = (byte)Mathf.Wrap(Mathf.RoundToInt(f), 8); }

        /// <summary>Creates a Direction closest to the bearing v from south.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(Vector2 v) { Value = (byte)Mathf.Step(Mathf.Bear(v.X, v.Y), 8, 360); }

        /// <summary>Creates a Direction closest to the bearing between (0,-1) and v.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(Vector3 v) { Value = (byte)Mathf.Step(Mathf.Bear(v.X, v.Y), 8, 360); }


        /// <summary>Creates a Direction closest to the bearing between (0,-1) and (x,y).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(float x, float y) { Value = (byte)Mathf.Step(Mathf.Bear(x, y), 8, 360); }

        /// <summary>Creates a Direction closest to the bearing between vectors a and b.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(Vector2 a, Vector2 b) { Value = (byte)Mathf.Step(Mathf.Bear(a, b), 8, 360); }

        /// <summary>Creates a Direction closest to the bearing between vectors a and b.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(Vector3 a, Vector3 b) { Value = (byte)Mathf.Step(Mathf.Bear(a, b), 8, 360); }


        /// <summary>Creates a Direction closest to the bearing between (0,-1) and (x,y).</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction(int x, int y) { Value = (byte)Mathf.Step(Mathf.Bear(x, y), 8, 360); }


        /// <summary>Returns the x-leg of the Direction vector as 0, 1 or -1.</summary>
        public int X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (Value)
                {
                    case 0: case 4: return 0;
                    case 1: case 2: case 3: return 1;
                    case 5: case 6: case 7: return -1;
                    default: return 0;
                }
            }
        }

        /// <summary>Returns the y-leg of the Direction vector as 0, 1 or -1.</summary>
        public int Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (Value)
                {
                    case 2: case 6: return 0;
                    case 3: case 4: case 5: return -1;
                    case 0: case 1: case 7: return 1;
                    default: return 0;
                }
            }
        }


        /// <summary>Creates a Direction facing south.</summary>
        public static Direction S
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)0; }
        }

        /// <summary>Creates a Direction facing southeast.</summary>
        public static Direction SE
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)1; }
        }

        /// <summary>Creates a Direction facing east.</summary>
        public static Direction E
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)2; }
        }

        /// <summary>Creates a Direction facing northeast.</summary>
        public static Direction NE
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)3; }
        }

        /// <summary>Creates a Direction facing north.</summary>
        public static Direction N
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)4; }
        }

        /// <summary>Creates a Direction facing northwest.</summary>
        public static Direction NW
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)5; }
        }

        /// <summary>Creates a Direction facing west.</summary>
        public static Direction W
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)6; }
        }

        /// <summary>Creates a Direction facing southwest.</summary>
        public static Direction SW
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (byte)7; }
        }


        /// <summary>Implicitly converts a Direction to a byte.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator byte(Direction d) { return d.Value; }

        /// <summary>Implicitly converts a byte b to a Direction equal to b % 8.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Direction(byte b) { return new Direction(b); }

        /// <summary>Implicitly converts an integer i to a Direction equal to i % 8.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Direction(int i) { return new Direction(i); }

        /// <summary>Explicitly converts a float f to a Direction where f is an bearing between 0 and 360.</summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Direction(float f) { return new Direction(f); }

        /// <summary>Explicitly converts a Vector2 vector to the closest Direction.</summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Direction(Vector2 v) { return new Direction(v); }

        /// <summary>Explicitly converts a Vector3 vector to the closest Direction.</summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Direction(Vector3 v) { return new Direction(v); }


        /// <summary>Returns the result of a modulus operation between Direction and int.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator %(Direction lhs, int rhs) { return lhs.Value % rhs; }

        /// <summary>Returns the result of a modulus operation between Direction and byte.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator %(Direction lhs, byte rhs) { return lhs.Value % rhs; }

        /// <summary>Returns the result of a addition operation on two Directions.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator +(Direction lhs, Direction rhs)
        {
            return new Direction(lhs.Value + rhs.Value);
        }

        /// <summary>Returns the result of a subtraction operation on two Directions.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator -(Direction lhs, Direction rhs)
        {
            return new Direction(lhs.Value - rhs.Value);
        }

        /// <summary>Returns the inverse Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator -(Direction d) { return new Direction(d + 4); }

        /// <summary>Returns the result of a unary addition operation of the Direction value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator +(Direction d) { return +d.Value; }


        /// <summary>Returns the Direction one step anticlockwise of the current Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator ++(Direction d) { return new Direction(++d.Value); }

        /// <summary>Returns the Direction one step clockwise of the current Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction operator --(Direction d) { return new Direction(--d.Value); }


        /// <summary>Returns the result of an equality operation on two Directions.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Direction lhs, Direction rhs) { return lhs.Value == rhs.Value; }

        /// <summary>Returns the result of an equality operation on a wrapped byte and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(byte lhs, Direction rhs) { return Mathf.Wrap(lhs, 8) == rhs.Value; }

        /// <summary>Returns the result of an equality operation on a wrapped byte and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Direction lhs, byte rhs) { return lhs.Value == Mathf.Wrap(rhs, 8); }

        /// <summary>Returns the result of an equality operation on a wrapped integer and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(int lhs, Direction rhs) { return Mathf.Wrap(lhs, 8) == rhs.Value; }

        /// <summary>Returns the result of an equality operation on a wrapped integer and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Direction lhs, int rhs) { return lhs.Value == Mathf.Wrap(rhs, 8); }

        /// <summary>Returns the result of an equality operation on a wrapped rounded float and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(float lhs, Direction rhs) { return Mathf.RoundToInt(lhs) % 8 == rhs.Value; }

        /// <summary>Returns the result of an equality operation on a wrapped rounded float and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Direction lhs, float rhs) { return lhs.Value == Mathf.RoundToInt(rhs) % 8; }


        /// <summary>Returns the result of an equality operation on the closest Direction to Vector2 and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2 lhs, Direction rhs) { return Mathf.Step(Mathf.Bear(lhs), 8, 360) == rhs.Value; }

        /// <summary>Returns the result of an equality operation on the closest Direction to Vector2 and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Direction lhs, Vector2 rhs) { return lhs.Value == Mathf.Step(Mathf.Bear(rhs), 8, 360); }


        /// <summary>Returns the result of an equality operation on the closest Direction to Vector3 and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 lhs, Direction rhs) { return Mathf.Step(Mathf.Bear(lhs), 8, 360) == rhs.Value; }

        /// <summary>Returns the result of an equality operation on the closest Direction to Vector3 and Direction.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Direction lhs, Vector3 rhs) { return lhs.Value == Mathf.Step(Mathf.Bear(rhs), 8, 360); }



        /// <summary>Returns the result of an inequality operation on two Directions.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Direction lhs, Direction rhs) { return lhs.Value != rhs.Value; }


        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(byte lhs, Direction rhs) { return Mathf.Wrap(lhs, 8) != rhs.Value; }

        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Direction lhs, byte rhs) { return lhs.Value != Mathf.Wrap(rhs, 8); }


        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(int lhs, Direction rhs) { return Mathf.Wrap(lhs, 8) != rhs.Value; }

        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Direction lhs, int rhs) { return lhs.Value != Mathf.Wrap(rhs, 8); }


        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(float lhs, Direction rhs) { return Mathf.RoundToInt(lhs) % 8 != rhs.Value; }

        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Direction lhs, float rhs) { return lhs.Value != Mathf.RoundToInt(rhs) % 8; }


        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2 lhs, Direction rhs) { return Mathf.Step(Mathf.Bear(lhs), 8, 360) != rhs.Value; }

        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Direction lhs, Vector2 rhs) { return lhs.Value != Mathf.Step(Mathf.Bear(rhs), 8, 360); }


        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 lhs, Direction rhs) { return Mathf.Step(Mathf.Bear(lhs), 8, 360) != rhs.Value; }

        /// <summary>Returns the result of an inequality operation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Direction lhs, Vector3 rhs) { return lhs.Value != Mathf.Step(Mathf.Bear(rhs), 8, 360); }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Direction rhs) { return this.Value == rhs.Value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public bool Equals(byte rhs) { return this.Value == Mathf.Wrap(rhs, 8); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public override bool Equals(object o) { return Equals((Direction)o); }


        /// <summary>Returns a hash code for the Direction equal to its byte value's hashcode.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() { return Value.GetHashCode(); }

        /// <summary>Wraps the CompareTo function of the value byte.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(byte other) { return Value.CompareTo(other); }

        /// <summary> Returns the name of the Direction as a string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            switch (Value)
            {
                case 0: return "S";
                case 1: return "SE";
                case 2: return "E";
                case 3: return "NE";
                case 4: return "N";
                case 5: return "NW";
                case 6: return "W";
                case 7: return "SW";
                default: throw new NotSupportedException();
            }
        }

        /// <summary> Returns the name of the Direction as a string.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }

    public static partial class Mathf
    {
        ///<summary> Returns the bearing from south to Direction b. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Bear(Direction b) { return b * 45f; }

        ///<summary> Returns Direction a clamped to focus on Direction b, within 1 Direction. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction Focus(Direction a, Direction b)
        {
            return a - b == 0 || a - b == 4 ? b : a - b < 4 ? b + 1 : b - 1;
        }
    }
}