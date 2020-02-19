// common storage location for generic enums

namespace Nez
{
    public enum HorizontalAlign
    {
        Left,
        Center,
        Right
    }


    public enum VerticalAlign
    {
        Top,
        Center,
        Bottom
    }


    public enum Edge
    {
        Top,
        Bottom,
        Left,
        Right
    }


    public enum InputDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum LoopMode
    {
        /// <summary>
        /// Play the sequence in a loop forever [A][B][C][A][B][C][A][B][C]...
        /// </summary>
        Loop,

        /// <summary>
        /// Play the sequence once [A][B][C] then pause and set time to 0 [A]
        /// </summary>
        Once,

        /// <summary>
        /// Plays back the animation once, [A][B][C]. When it reaches the end, it will keep playing the last frame and never stop playing
        /// </summary>
        ClampForever,

        /// <summary>
        /// Play the sequence in a ping pong loop forever [A][B][C][B][A][B][C][B]...
        /// </summary>
        PingPong,

        /// <summary>
        /// Play the sequence once forward then back to the start [A][B][C][B][A] then pause and set time to 0
        /// </summary>
        PingPongOnce
    }

	


}